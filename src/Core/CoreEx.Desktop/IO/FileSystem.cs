﻿namespace More.IO
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a file system that provides access to folders and files.
    /// </summary>
    public class FileSystem : IFileSystem
    {
        /// <summary>
        /// Gets the folder that has the specified path in the file system.
        /// </summary>
        /// <param name="path">The path of the folder to retrieve.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing the retreived <see cref="IFolder">folder</see>.</returns>
        public Task<IFolder> GetFolderAsync( string path )
        {
            var directory = new DirectoryInfo( path );
            return Task.FromResult( directory.AsFolder() );
        }

        /// <summary>
        /// Gets the file that has the specified path in the file system.
        /// </summary>
        /// <param name="path">The path of the file to retrieve.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing the retreived <see cref="IFile">file</see>.</returns>
        public Task<IFile> GetFileAsync( string path )
        {
            var file = new FileInfo( path );
            return Task.FromResult( file.AsFile() );
        }

        /// <summary>
        /// Gets the file that has the specified Uniform Resource Indicator (URI) in the file system.
        /// </summary>
        /// <param name="uri">The <see cref="Uri">URI</see> of the file to retrieve.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing the retreived <see cref="IFile">file</see>.</returns>
        public Task<IFile> GetFileAsync( Uri uri )
        {
            if ( !uri.IsFile )
                throw new FileNotFoundException( SR.InvalidFileUri );

            var file = new FileInfo( uri.LocalPath );
            return Task.FromResult( file.AsFile() );
        }
    }
}