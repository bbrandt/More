﻿namespace More
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Diagnostics.CodeAnalysis;
    using global::System.Diagnostics.Contracts;
    using global::System.Linq;

    /// <summary>
    /// Represents a <see cref="IServiceProvider">service provider</see> that can adapt to another
    /// service locator or dependency injection interface.
    /// </summary>
    public class ServiceProviderAdapter : IServiceProvider
    {
        private readonly Lazy<ServiceTypeDisassembler> disassembler = new Lazy<ServiceTypeDisassembler>( () => new ServiceTypeDisassembler() );
        private readonly Func<Type, string, object> resolve;
        private readonly Func<Type, string, IEnumerable<object>> resolveAll;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceProviderAdapter"/> class.
        /// </summary>
        /// <param name="resolve">The <see cref="Func{T1,T2,TResult}">function</see> used to resolve a single service of a particular <see cref="Type">type</see>.</param>
        public ServiceProviderAdapter( Func<Type, string, object> resolve )
        {
            Contract.Requires<ArgumentNullException>( resolve != null, "resolve" );

            this.resolve = resolve;
            this.resolveAll = this.DefaultResolveAll;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceProviderAdapter"/> class.
        /// </summary>
        /// <param name="resolve">The <see cref="Func{T1,T2,TResult}">function</see> used to resolve a single service of a particular <see cref="Type">type</see>.</param>
        /// <param name="resolveAll">The <see cref="Func{T1,T2,TResult}">function</see> used to resolve all services of a particular <see cref="Type">type</see>.</param>
        public ServiceProviderAdapter( Func<Type, string, object> resolve, Func<Type, string, IEnumerable<object>> resolveAll )
        {
            Contract.Requires<ArgumentNullException>( resolve != null, "resolve" );
            Contract.Requires<ArgumentNullException>( resolveAll != null, "resolveAll" );

            this.resolve = resolve;
            this.resolveAll = resolveAll;
        }

        private ServiceTypeDisassembler Disassembler
        {
            get
            {
                Contract.Ensures( Contract.Result<ServiceTypeDisassembler>() != null );
                return this.disassembler.Value;
            }
        }

        [SuppressMessage( "Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "This method should not throw exceptions for service resolution failures." )]
        private IEnumerable<object> DefaultResolveAll( Type serviceType, string key )
        {
            Contract.Requires( serviceType != null );

            var services = new List<object>();

            if ( key == null )
            {
                // add ourself when requested
                if ( typeof( IServiceProvider ) == serviceType )
                    services.Add( this );
            }

            try
            {
                // return sequence with a single element
                var service = this.resolve( serviceType, key );

                if ( service != null )
                    services.Add( service );
            }
            catch
            {
                // do nothing if resolution fails
            }

            return services;
        }

        /// <summary>
        /// Gets a service of the requested type.
        /// </summary>
        /// <param name="serviceType">The <see cref="Type">type</see> of service to return.</param>
        /// <returns>The service instance corresponding to the requested
        /// <paramref name="serviceType">service type</paramref> or null if no match is found.</returns>
        public virtual object GetService( Type serviceType )
        {
            if ( serviceType == null )
                throw new ArgumentNullException( "serviceType" );

            var key = this.Disassembler.ExtractKey( serviceType );
            Type innerServiceType;

            // return multiple services, if requested
            if ( this.Disassembler.IsForMany( serviceType, out innerServiceType ) )
                return this.resolveAll( innerServiceType, key );

            if ( key == null )
            {
                // return ourself when requested
                if ( typeof( IServiceProvider ) == serviceType )
                    return this;
            }

            return this.resolve( serviceType, key );
        }
    }
}