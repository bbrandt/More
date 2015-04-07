﻿namespace More.ComponentModel
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Diagnostics.CodeAnalysis;
    using global::System.Diagnostics.Contracts;
    using global::System.Reflection;

    /// <summary>
    /// Represents a edit transation for object field members.
    /// </summary>
    public class FieldTransaction : MemberTransaction<FieldInfo>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FieldTransaction"/> class.
        /// </summary>
        /// <param name="target">The target <see cref="Object">object</see> for the transaction.</param>
        /// <remarks>All public, supported fields are enlisted.</remarks>
        public FieldTransaction( object target )
            : base( target )
        {
            Contract.Requires<ArgumentNullException>( target != null, "target" );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldTransaction"/> class.
        /// </summary>
        /// <param name="target">The target <see cref="Object">object</see> for the transaction.</param>
        /// <param name="editSnapshotStrategy">The <see cref="IEditSnapshotStrategy">edit snapshot strategy</see> used to
        /// snapshot savepoint values.</param>
        /// <remarks>All public, supported fields are enlisted.</remarks>
        public FieldTransaction( object target, IEditSnapshotStrategy editSnapshotStrategy )
            : base( target, editSnapshotStrategy )
        {
            Contract.Requires<ArgumentNullException>( target != null, "target" );
            Contract.Requires<ArgumentNullException>( editSnapshotStrategy != null, "editSnapshotStrategy" );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldTransaction"/> class.
        /// </summary>
        /// <param name="target">The target <see cref="Object">object</see> for the transaction.</param>
        /// <param name="fieldNames">A <see cref="IEnumerable{FieldInfo}">sequence</see> of field names to enlist in the transaction.</param>
        public FieldTransaction( object target, IEnumerable<string> fieldNames )
            : base( target, fieldNames )
        {
            Contract.Requires<ArgumentNullException>( target != null, "target" );
            Contract.Requires<ArgumentNullException>( fieldNames != null, "fieldNames" );
            Contract.Requires<ArgumentNullException>( Contract.ForAll( fieldNames, memberName => !string.IsNullOrEmpty( memberName ) ), "fieldNames[]" );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldTransaction"/> class.
        /// </summary>
        /// <param name="target">The target <see cref="Object">object</see> for the transaction.</param>
        /// <param name="fieldNames">A <see cref="IEnumerable{FieldInfo}">sequence</see> of field names to enlist in the transaction.</param>
        /// <param name="editSnapshotStrategy">The <see cref="IEditSnapshotStrategy">edit snapshot strategy</see> used to
        /// snapshot savepoint values.</param>
        public FieldTransaction( object target, IEnumerable<string> fieldNames, IEditSnapshotStrategy editSnapshotStrategy )
            : base( target, fieldNames, editSnapshotStrategy )
        {
            Contract.Requires<ArgumentNullException>( target != null, "target" );
            Contract.Requires<ArgumentNullException>( fieldNames != null, "fieldNames" );
            Contract.Requires<ArgumentNullException>( editSnapshotStrategy != null, "editSnapshotStrategy" );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldTransaction"/> class.
        /// </summary>
        /// <param name="target">The target <see cref="Object">object</see> for the transaction.</param>
        /// <param name="filter">A <see cref="Func{T1,TResult}">function</see> used to filter transacted fields.</param>
        public FieldTransaction( object target, Func<FieldInfo, bool> filter )
            : base( target, filter )
        {
            Contract.Requires<ArgumentNullException>( target != null, "target" );
            Contract.Requires<ArgumentNullException>( filter != null, "filter" );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldTransaction"/> class.
        /// </summary>
        /// <param name="target">The target <see cref="Object">object</see> for the transaction.</param>
        /// <param name="filter">A <see cref="Func{T1,TResult}">function</see> used to filter transacted fields.</param>
        /// <param name="editSnapshotStrategy">The <see cref="IEditSnapshotStrategy">edit snapshot strategy</see> used to
        /// snapshot savepoint values.</param>
        public FieldTransaction( object target, Func<FieldInfo, bool> filter, IEditSnapshotStrategy editSnapshotStrategy )
            : base( target, filter, editSnapshotStrategy )
        {
            Contract.Requires<ArgumentNullException>( target != null, "target" );
            Contract.Requires<ArgumentNullException>( filter != null, "filter" );
            Contract.Requires<ArgumentNullException>( editSnapshotStrategy != null, "editSnapshotStrategy" );
        }

        /// <summary>
        /// Returns the type for the specified field.
        /// </summary>
        /// <param name="member">The <see cref="FieldInfo">field</see> to get the <see cref="Type">type</see> for.</param>
        /// <returns>A <see cref="Type">type</see> object.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by code contract." )]
        protected override Type GetMemberType( FieldInfo member )
        {
            return member.FieldType;
        }

        /// <summary>
        /// Gets the value of the specified field.
        /// </summary>
        /// <param name="member">The <see cref="FieldInfo">field</see> to get the value for.</param>
        /// <returns>The value of the member.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by code contract." )]
        protected override object GetMemberValue( FieldInfo member )
        {
            return member.GetValue( this.Instance );
        }

        /// <summary>
        /// Sets the value of the specified field.
        /// </summary>
        /// <param name="member">The <see cref="FieldInfo">field</see> to set the value for.</param>
        /// <param name="value">The value to set.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by code contract." )]
        protected override void SetMemberValue( FieldInfo member, object value )
        {
            member.SetValue( this.Instance, value );
        }
    }
}
