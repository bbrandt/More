﻿namespace More.VisualStudio.Views
{
    using Microsoft.VisualStudio.Data.Services;
    using Microsoft.VisualStudio.Shell.Interop;
    using More.VisualStudio.ViewModels;
    using More.Windows.Input;
    using System;
    using System.Diagnostics.Contracts;
    using System.Windows;
    using System.Windows.Input;
    using System.Xml;

    /// <summary>
    /// Represents the view for an Entity Framework (EF) DbContext item template wizard.
    /// </summary>
    public partial class DbContextItemTemplateWizard : Window
    {
        private readonly IVsUIShell shell;
        private readonly ProjectInformation projectInfo;
        private readonly Lazy<IVsDataConnectionDialogFactory> dataConnectionDialogFactory;
        private readonly Lazy<IVsDataExplorerConnectionManager> dataExplorerConnectionManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="DbContextItemTemplateWizard"/> class.
        /// </summary>
        public DbContextItemTemplateWizard()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbContextItemTemplateWizard"/> class.
        /// </summary>
        /// <param name="model">The <see cref="ViewItemTemplateWizardViewModel">model</see> for the view.</param>
        /// <param name="projectInformation">The source <see cref="ProjectInformation">project information</see> used for browsing existing types.</param>
        /// <param name="shell">The <see cref="IVsUIShell">shell</see> used to provide user feedback.</param>
        /// <param name="dataExplorerConnectionManager">The <see cref="Lazy{T}">on-demand</see> <see cref="IVsDataConnectionDialogFactory"/> used to create new data connections.</param>
        /// <param name="dataConnectionDialogFactory">The <see cref="Lazy{T}">on-demand</see> <see cref="IVsDataExplorerConnectionManager">data explorer connection manager</see> used to add created data connections.</param>
        public DbContextItemTemplateWizard(
            DbContextItemTemplateWizardViewModel model,
            ProjectInformation projectInformation,
            IVsUIShell shell,
            Lazy<IVsDataConnectionDialogFactory> dataConnectionDialogFactory,
            Lazy<IVsDataExplorerConnectionManager> dataExplorerConnectionManager )
        {
            Contract.Requires<ArgumentNullException>( shell != null, "shell" );
            Contract.Requires<ArgumentNullException>( dataConnectionDialogFactory != null, "dialogFactory" );
            Contract.Requires<ArgumentNullException>( dataExplorerConnectionManager != null, "dataExplorerConnectionManager" );

            this.InitializeComponent();
            this.Model = model;
            this.projectInfo = projectInformation;
            this.shell = shell;
            this.dataConnectionDialogFactory = dataConnectionDialogFactory;
            this.dataExplorerConnectionManager = dataExplorerConnectionManager;
        }

        /// <summary>
        /// Gets or sets the model associated with the view.
        /// </summary>
        /// <value>The associated <see cref="DbContextItemTemplateWizardViewModel">model</see>.</value>
        public DbContextItemTemplateWizardViewModel Model
        {
            get
            {
                return this.DataContext as DbContextItemTemplateWizardViewModel;
            }
            set
            {
                var oldValue = this.Model;
                this.UnwireInteractionRequests( oldValue );
                this.DataContext = value;
                this.WireInteractionRequests( value );
            }
        }

        /// <summary>
        /// Gets the factory for creating data connection dialogs.
        /// </summary>
        /// <value>A <see cref="IVsDataConnectionDialogFactory">data connection dialog factory</see>.</value>
        protected IVsDataConnectionDialogFactory DataConnectionDialogFactory
        {
            get
            {
                Contract.Ensures( Contract.Result<IVsDataConnectionDialogFactory>() != null );
                return this.dataConnectionDialogFactory.Value;
            }
        }

        /// <summary>
        /// Gets the manager for data explorer connections.
        /// </summary>
        /// <value>A <see cref="IVsDataExplorerConnectionManager">data explorer connection manager</see>.</value>
        protected IVsDataExplorerConnectionManager DataExplorerConnectionManager
        {
            get
            {
                Contract.Ensures( Contract.Result<IVsDataExplorerConnectionManager>() != null );
                return this.dataExplorerConnectionManager.Value;
            }
        }

        /// <summary>
        /// Shows an error message to the user.
        /// </summary>
        /// <param name="title">The title of message.</param>
        /// <param name="text">The error message text.</param>
        protected void ShowError( string title, string text )
        {
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( title ), "title" );
            Contract.Requires<ArgumentNullException>( text != null, "text" );
            this.shell.ShowError( title, text );
        }

        private void WireInteractionRequests( DbContextItemTemplateWizardViewModel model )
        {
            if ( model == null )
                return;

            model.InteractionRequests["BrowseForModel"].Requested += this.OnBrowseForModel;
            model.InteractionRequests["AddDataConnection"].Requested += this.OnAddDataConnection;
        }

        private void UnwireInteractionRequests( DbContextItemTemplateWizardViewModel model )
        {
            if ( model == null )
                return;

            model.InteractionRequests["BrowseForModel"].Requested -= this.OnBrowseForModel;
            model.InteractionRequests["AddDataConnection"].Requested -= this.OnAddDataConnection;
        }

        private async void OnBrowseForModel( object sender, InteractionRequestedEventArgs e )
        {
            Contract.Requires( e != null );

            var picker = new TypePicker()
            {
                Title = e.Interaction.Title,
                LocalAssemblyName = this.Model.LocalAssemblyName,
                SourceProject = this.projectInfo,
                RestrictedBaseTypeNames =
                {
                    "System.Windows.DependencyObject",
                    "System.Data.Entity.DbContext"
                }
            };

            if ( await picker.ShowDialogAsync( this ) ?? false )
            {
                this.Model.ModelType = picker.SelectedType;
                e.Interaction.ExecuteDefaultCommand();
            }
            else
            {
                e.Interaction.ExecuteCancelCommand();
            }
        }

        private void OnAddDataConnection( object sender, InteractionRequestedEventArgs e )
        {
            // ask visual studio to create a dialog
            using ( var dialog = this.DataConnectionDialogFactory.CreateConnectionDialog() )
            {
                dialog.AddSources( null );
                dialog.LoadSourceSelection();

                // display the dialog
                var connection = dialog.ShowDialog( true );

                // if no connection is returned, the dialog was cancelled
                if ( connection == null )
                {
                    e.Interaction.ExecuteCancelCommand();
                    return;
                }

                // save the selections
                if ( dialog.SaveSelection )
                {
                    dialog.SaveProviderSelections();
                    dialog.SaveProviderSelections();
                }

                IVsDataExplorerConnection dataExplorerConnection = null;

                try
                {
                    // add the connection to the data explorer
                    dataExplorerConnection = this.DataExplorerConnectionManager.AddConnection( null, connection.Provider, connection.EncryptedConnectionString, true );
                }
                catch ( XmlException ex )
                {
                    this.ShowError( e.Interaction.Title, ExceptionMessage.DataConnectionInvalid.FormatDefault( ex.Message ) );
                    return;
                }

                // create a new data source
                var dataSource = new DataSource( dataExplorerConnection.DisplayName, connection );
                var command = e.Interaction.DefaultCommand;

                // invoke the accept command, supplying the new data source
                if ( command != null && command.CanExecute( dataSource ) )
                    command.Execute( dataSource );
            }
        }
    }
}
