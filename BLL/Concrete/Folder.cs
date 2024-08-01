using BLL.Abstract;
using BLL.Utils;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using static BLL.Concrete.Folder;
using Serilog;
using System.Linq;
using System.IO;

namespace BLL.Concrete
{
    public class Folder : IFolder
    {
        private string Path;
        private string ReplicaPath;
        private List<Folder> Folders;
        private List<string> Files;
        private List<FolderCheckers> Checkers;
        private Folder(string path, string replicaPath, params FolderCheckers[] folderCheckers)
        {
            this.Path = path;
            this.ReplicaPath = replicaPath;
            this.Checkers = folderCheckers.ToList();
            this.Folders = new List<Folder>();
            this.Files = new List<string>();
        }
        public void CheckBasedOnCheckers(Folder folder = null)
        {
            if (folder == null)
            {
                folder = this;
            }

            if (folder.Checkers.Contains(FolderCheckers.Deleted))
            {
                CheckDeleted(folder);
                CheckDeletedFolder(folder);
            }
            if (folder.Checkers.Contains(FolderCheckers.Created))
            {
                CheckCreatedFolders(folder);
                CheckCreated(folder);
            }
            if (folder.Checkers.Contains(FolderCheckers.Updated))
            {
                CheckUpdated(folder);
            }

            /*foreach (var folderItem in folder.Folders)
            {
                CheckBasedOnCheckers(folderItem);
            }*/

            
        }
        private void CheckCreatedFolders(Folder folder)
        {
            if (!Directory.Exists(folder.Path))
            {
                return;
            }

            if (!Directory.Exists(folder.Path.Replace(Path, ReplicaPath)))
            {
                Directory.CreateDirectory(folder.Path.Replace(Path, ReplicaPath));
            }
            foreach (var folderItem in Directory.GetDirectories(folder.Path).ToList())
            {
                if (!folder.Folders.Select(x => x.Path).Contains(folderItem))
                {
                    Log.Information(FolderCheckers.Created.ToString() + ": " + folderItem);
                    Directory.CreateDirectory(folderItem.Replace(Path, ReplicaPath));
                    folder.Folders.Add(new Folder(folderItem, folderItem.Replace(this.Path, this.ReplicaPath), folder.Checkers.ToArray()));
                }
            }

            foreach(var folderItem in folder.Folders)
            {
                CheckCreatedFolders(folderItem);
            }
        }
        public void CheckCreated(Folder folder)
        {
            if (!Directory.Exists(folder.Path))
            {
                return;
            }

            List<string> createdFiles = new List<string>();
            foreach (var file in Directory.GetFiles(folder.Path))
            {
                if (!folder.Files.Contains(file))
                {
                    Log.Information(FolderCheckers.Created.ToString() + ": " + file);
                    File.Copy(file, file.Replace(folder.Path, folder.ReplicaPath));
                    createdFiles.Add(file);
                }
            }
            folder.Files.AddRange(createdFiles);

            foreach (var folderItem in folder.Folders)
            {
                CheckCreated(folderItem);
            }
        }
        public void CheckDeletedFolder(Folder folder)
        {
            List<string> mustBeRemovedFolders = new List<string>();
            foreach (var folderItem in folder.Folders)
            {
                CheckDeletedFolder(folderItem);

                if (!Directory.GetDirectories(folder.Path).Contains(folderItem.Path))
                {
                    Log.Information(FolderCheckers.Deleted.ToString() + ": " + folderItem.Path);
                    if (Directory.Exists(folderItem.Path.Replace(Path, ReplicaPath)))
                    {
                        Directory.Delete(folderItem.Path.Replace(Path, ReplicaPath));
                    }
                }
            }
            folder.Folders = folder.Folders.Where(x => !mustBeRemovedFolders.Contains(x.Path)).ToList();
        }
        public void CheckDeleted(Folder folder)
        {
            List<string> deletedFiles = new List<string>();
            foreach (var file in folder.Files)
            {
                if (!Directory.Exists(folder.Path) || !Directory.GetFiles(folder.Path).Contains(file))
                {
                    Log.Information(FolderCheckers.Deleted.ToString() + ": " + file);
                    File.Delete(file.Replace(folder.Path, folder.ReplicaPath));
                    deletedFiles.Add(file);
                }
            }
            folder.Files = folder.Files.Where(x => !deletedFiles.Contains(x)).ToList();

            foreach (var folderItem in folder.Folders)
            {
                CheckDeleted(folderItem);
            }
        }
        public void CheckUpdated(Folder folder)
        {
            foreach (var file in folder.Files)
            {
                if (!EqualFiles(file, file.Replace(folder.Path, folder.ReplicaPath)))
                {
                    //File.Move(file, file.Replace(folder.Path, folder.ReplicaPath));

                    Log.Information(FolderCheckers.Updated.ToString() + ": " + file);
                    File.Delete(file.Replace(folder.Path, folder.ReplicaPath));
                    File.Copy(file, file.Replace(folder.Path, folder.ReplicaPath));
                }
            }
        }
        private bool EqualFiles(string sourceFile, string replicaFile)
        {
            using (var md5 = MD5.Create())
            {
                using (var streamSourceFile = File.OpenRead(sourceFile))
                using (var streamReplicaFile = File.OpenRead(replicaFile))
                {
                    var hashSourceFile = md5.ComputeHash(streamSourceFile);
                    var hashReplicaFile = md5.ComputeHash(streamReplicaFile);

                    return BitConverter.ToString(hashSourceFile) == BitConverter.ToString(hashReplicaFile);
                }
            }
        }
        public static FolderBuilder builder()
        {
            return new FolderBuilder();
        }
        public class FolderBuilder : IFolderBuilder
        {
            private List<FolderCheckers> BuiilderFolderCheckers = new List<FolderCheckers>();
            private string Root;
            private string ReplicaRoot;
            public Folder Build()
            {
                foreach(var file in Directory.GetFiles(ReplicaRoot))
                {
                    File.Delete(file);
                }
                foreach(var folder in Directory.GetDirectories(ReplicaRoot))
                {
                    foreach (var file in Directory.GetFiles(folder))
                    {
                        File.Delete(file);
                    }
                    Directory.Delete(folder);
                }
                return new Folder(Root, ReplicaRoot, BuiilderFolderCheckers.ToArray());
            }
            public FolderBuilder CheckForCreate()
            {
                BuiilderFolderCheckers.Add(FolderCheckers.Created);
                return this;
            }
            public FolderBuilder CheckForDelete()
            {
                BuiilderFolderCheckers.Add(FolderCheckers.Deleted);
                return this;
            }
            public FolderBuilder CheckForUpdate()
            {
                BuiilderFolderCheckers.Add(FolderCheckers.Updated);
                return this;
            }
            public FolderBuilder SetRoot(string root)
            {
                Root = root;
                return this;
            }
            public FolderBuilder SetReplicaRoot(string replicaRoot)
            {
                ReplicaRoot = replicaRoot;
                return this;
            }

        }

    }
}
