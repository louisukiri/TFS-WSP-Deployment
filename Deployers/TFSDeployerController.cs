using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Deployers.Concrete.SqlDeployer;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using toDev.Abstract;
using toDev.Concrete.ZipDeployer;

namespace Deployers
{
    public static class TFSSettings
    {
        internal static string TFSServerURL{get;set;}
        internal static List<string> BaseTFSDirectories { get; set; }
    }
    public class TFSDeployerController : DeployerController
    {
        protected BuildConfiguration _bldConfig;
        protected string zipDeploymentDir { get; set; }
        protected List<string> tfsBaseDir { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bldConfig"></param>
        /// <param name="buildNumber"></param>
        public TFSDeployerController(string TFSServerAddress, string bldConfigDir, string buildNumber)
        {
            TFSSettings.TFSServerURL = TFSServerAddress;
            
            BuildConfiguration bldConfig = this._configuration= BuildConfiguration.GetConfig(bldConfigDir);
            this.setConfiguration(bldConfig, buildNumber);
            TFSSettings.BaseTFSDirectories = (bldConfig != null && bldConfig.Rules["BaseTFSDir"] != null) ? bldConfig.Rules["BaseTFSDir"].Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList() : new List<string> { baseDir };
            _bldConfig = bldConfig;
            zipDeploymentDir = (bldConfig != null && bldConfig.Rules["BaseZipDeploymentDir"] != null)?bldConfig.Rules["BaseZipDeploymentDir"].Value : buildDir;
            tfsBaseDir = TFSSettings.BaseTFSDirectories;
        }
        public TFSDeployerController(string TFSServerAddress, string baseDir, string DeploymentDir, string buildNumber)
            : base(baseDir, DeploymentDir, buildNumber)
        {
            TFSSettings.TFSServerURL = TFSServerAddress;
            TFSSettings.BaseTFSDirectories = new List<string> { baseDir };
        }
        public string deploy(TfsTeamProjectCollection tfs, IList<Changeset> changesets)
        {
            return deploy(tfs, changesets, (_bldConfig != null)?_bldConfig.Rules["BuildDeploymentDir"].Value : null);
        }
        public string deploy(TfsTeamProjectCollection tfs, IList<Changeset> changesets, string deployerBuildDirectory)
        {
            VersionControlServer vcs = (VersionControlServer)tfs.GetService(typeof(VersionControlServer));
            string logString = "Deploying "+ base.buildNumber.ToString();

            //commensurate the zip deployment with the current build
            TeamFoundationIdentity id;
            tfs.GetAuthenticatedIdentity(out id);

                foreach (var changesetSummary in changesets.OrderBy(a=>a.ChangesetId))
                {
                    Changeset changeSet = vcs.GetChangeset(changesetSummary.ChangesetId);
                    var b = changeSet.Changes.Where(c =>
                        c.Item.ItemType == ItemType.File
                       )
                       .Where(d=> !d.Item.ServerItem.EndsWith(".sql"))
                        .Select(a => a.Item.ServerItem)
                        .ToList();

                    string committer = changeSet.Committer;
                    string comment = changeSet.Comment;
 
                    if (b.Count > 0)
                    {
                       string depDir = zipDeploymentDir + "\\" + Regex.Replace(changesetSummary.Committer, @"\W", "_")+"\\";
                       
                       logString += DeployZip(b, depDir, committer, comment);
                    }
                    if (!string.IsNullOrEmpty(_bldConfig.ConnectionString))
                    { 
                        var e = changeSet.Changes.Where(c =>
                            c.Item.ItemType == ItemType.File
                           )
                           .Where(d => d.Item.ServerItem.EndsWith(".sql"))
                            .OrderBy(f => f.Item.ServerItem)
                            .Select(a => a.Item.ServerItem)
                            .ToList()
                            ;
                        if (e.Count > 0)
                        {
                            //try
                            //{
                                string depDir = (_bldConfig.Rules["SqlDeploymentDir"] != null && !string.IsNullOrEmpty(_bldConfig.Rules["SqlDeploymentDir"].Value)) ? _bldConfig.Rules["SqlDeploymentDir"].Value + "\\" + Regex.Replace(changesetSummary.Committer, @"\W", "_") + "\\" : buildDir;
                                SqlDeployer sqldeployer = new SqlDeployer(_bldConfig.ConnectionString);
                                sqldeployer.SourceDir = new List<string>{ baseDir};
                                sqldeployer.DeploymentDir = depDir;
                                sqldeployer.BuildNumber = buildNumber;
                                sqldeployer.SetComment(comment);
                                sqldeployer.LoadFiles(e);
                                logString += sqldeployer.Deploy();
                                //SqlDeploymentDir

                               //logString += DeployZip(e, depDir, committer, comment);
                            //}
                            //catch (Exception ex)
                            //{
                                
                            //}
                        }
                    }

                }

            return logString;
        
        }

        private string DeployZip(List<string> b, string zipdepDir, string committer, string comment)
        {
            if (!Directory.Exists(zipdepDir)) Directory.CreateDirectory(zipdepDir);
            //send the zipdeployer the tfsdir as base directory so that it can determine the 
            //virtual url based on the path
            //the base source directory is TFS
            ZipDeploy ZipDeployer2 = new ZipDeploy(tfsBaseDir, baseDir, zipdepDir, buildNumber);

            //run zip deployer methods first so that zip is ready even if directory write fails
            ZipDeployer2.UID = committer;
            ZipDeployer2.SetComment(comment);

            ZipDeployer2.LoadFiles(b);

            return ZipDeployer2.Deploy();
        }
    }
}
