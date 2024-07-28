using BLL.Abstract;
using BLL.Utils;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BLL.Concrete.Folder;

namespace BLL.Concrete
{
    public class Folder : IFolder
    {
        //DI
        private readonly static ILogger<Folder> logger;

        private string Path;
        private string ReplicaPath;
        private List<Folder> Folders;
        private List<string> Files;
        private List<FolderCheckers> Checkers;
        private Folder(string path, params FolderCheckers[] folderCheckers)
        {
            this.Path = path;
            this.Folders = new List<Folder>();
            foreach(var folder in Directory.GetDirectories(path).ToList())
            {
                this.Folders.Add(new Folder(folder!, folderCheckers));
            }
            this.Files = Directory.GetFiles(path).ToList();
            this.Checkers = folderCheckers.ToList();
        }

        public void CheckBasedOnCheckers()
        {
            switch(true)
            {
                case true when this.Checkers.Contains(FolderCheckers.Created):
                    CheckCreated();
                    break;
                case true when this.Checkers.Contains(FolderCheckers.Deleted):
                    CheckDeleted();
                    break;
                case true when this.Checkers.Contains(FolderCheckers.Updated):
                    CheckUpdated();
                    break;
            }
        }
        public void CheckCreated()
        {
            foreach (var file in Directory.GetFiles(Path))
            {
                if (!this.Files.Contains(file))
                {
                    logger.LogInformation(FolderCheckers.Created.ToString() + ": " + file);
                    //File.Copy(file,)
                }
            }
        }
        public void CheckDeleted()
        {

        }
        public void CheckUpdated()
        {

        }
        public class FolderBuilder : IFolderBuilder
        {

        }

    }
}
