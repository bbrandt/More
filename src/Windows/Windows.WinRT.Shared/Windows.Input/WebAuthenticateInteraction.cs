﻿namespace More.Windows.Input
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using global::Windows.Foundation.Collections;
    using global::Windows.Security.Authentication.Web;

    /// <summary>
    /// Represents an interaction request to perform a web-based authentication operation.
    /// </summary>
    public partial class WebAuthenticateInteraction : Interaction
    {
        private readonly Uri requestUri;
        private WebAuthenticationOptions options = WebAuthenticationOptions.None;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebAuthenticateInteraction"/> class.
        /// </summary>
        /// <param name="requestUri">The starting URI of the web service. This URI must be a secure address of https://.</param>
        public WebAuthenticateInteraction( Uri requestUri )
        {
            Contract.Requires<ArgumentNullException>( requestUri != null, "requestUri" );
            this.requestUri = requestUri;
        }

        private bool GetOption( WebAuthenticationOptions option )
        {
            return ( this.options & option ) == option;
        }

        private void SetOption( WebAuthenticationOptions option, bool value )
        {
            if ( value )
                this.options |= option;
            else
                this.options &= ~option;
        }

        /// <summary>
        /// Gets the starting Uniform Resource Identifier (URI) of the web service.
        /// </summary>
        /// <value>The starting <see cref="Uri">URI</see> of the web service. This
        /// <see cref="Uri">URI</see> must be a secure address of https://.</value>
        public Uri RequestUri
        {
            get
            {
                Contract.Ensures( this.requestUri != null );
                return this.requestUri;
            }
        }

        /// <summary>
        /// Gets or sets the callback Uniform Resource Indicator (URI) that indicates the completion of the web authentication. 
        /// </summary>
        /// <value>The callback <see cref="Uri">URI</see> that indicates the completion of the web authentication.
        /// The broker matches this <see cref="Uri">URI</see> against every <see cref="Uri">URI</see> that it is about to navigate to.</value>
        /// <remarks>The broker never navigates to this URI, instead the broker returns the control back to the application when the
        /// user clicks a link or a web server redirection is made.</remarks>
        public Uri CallbackUri
        {
            get;
            set;
        }

        internal WebAuthenticationOptions Options
        {
            get
            {
                return this.options;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the authentication operation uses corporate network authentication.
        /// </summary>
        /// <value>Tells the web authentication broker to render the webpage in an application container that
        /// supports privateNetworkClientServer, enterpriseAuthentication, and sharedUserCertificate capabilities.</value>
        /// <remarks>Note the application that uses this flag must have these capabilities as well.</remarks>
        public bool UseCorporateNetwork
        {
            get
            {
                return this.GetOption( WebAuthenticationOptions.UseCorporateNetwork );
            }
            set
            {
                this.SetOption( WebAuthenticationOptions.UseCorporateNetwork, value );
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the authentication operation returns the HTTP POST body in the response.
        /// </summary>
        /// <value>Tells the web authentication broker to return the body of the HTTP POST.</value>
        /// <remarks>For use with single sign-on (SSO) only.</remarks>
        public bool UseHttpPost
        {
            get
            {
                return this.GetOption( WebAuthenticationOptions.UseHttpPost );
            }
            set
            {
                this.SetOption( WebAuthenticationOptions.UseHttpPost, value );
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the authentication operation returns the window title in the response.
        /// </summary>
        /// <value>Tells the web authentication broker to return the window title string of the webpage.</value>
        public bool UseTitle
        {
            get
            {
                return this.GetOption( WebAuthenticationOptions.UseTitle );
            }
            set
            {
                this.SetOption( WebAuthenticationOptions.UseTitle, value );
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the operation authenticates using silent mode.
        /// </summary>
        /// <value>Tells the web authentication broker to not render any UI.</value>
        /// <remarks>For use with Single Sign On (SSO). If the server tries to display a webpage,
        /// the authentication operation fails.</remarks>
        public bool UseSilentMode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the response data when the operation completes successfully.
        /// </summary>
        /// <value>The success response data.</value>
        public string ResponseData
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the response status when the operation completes.
        /// </summary>
        /// <value>The response HTTP status code.</value>
        [CLSCompliant( false )]
        public uint ResponseStatus
        {
            get;
            set;
        }
    }
}
