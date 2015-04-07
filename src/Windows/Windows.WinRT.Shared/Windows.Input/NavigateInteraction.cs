﻿namespace More.Windows.Input
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Represents a user interaction navigation request.
    /// </summary>
    public class NavigateInteraction : Interaction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NavigateInteraction"/> class.
        /// </summary>
        public NavigateInteraction()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigateInteraction"/> class.
        /// </summary>
        /// <param name="title">The title associated with the interaction.</param>
        public NavigateInteraction( string title )
            : base( title )
        {
            Contract.Requires<ArgumentNullException>( title != null, "title" );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigateInteraction"/> class.
        /// </summary>
        /// <param name="title">The title associated with the interaction.</param>
        /// <param name="content">The content associated with the interaction.</param>
        public NavigateInteraction( string title, object content )
            : base( title, content )
        {
            Contract.Requires<ArgumentNullException>( title != null, "title" );
        }

        /// <summary>
        /// Gets or sets the Uniform Resource Locator (URL) of the page to navigate to.
        /// </summary>
        /// <value>The URL of the page to navigate to.</value>
        public Uri Url
        {
            get;
            set;
        }
    }
}
