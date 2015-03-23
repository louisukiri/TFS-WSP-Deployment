using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Microsoft.TeamFoundation.Build.Client;
using System.ServiceModel.Syndication;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using Ninject;
using Microsoft.TeamFoundation.Framework.Client;
using Deployers;

namespace stamats.buildhelper
{
    [BuildActivity(HostEnvironmentOption.All)]
    public sealed class BuildActivity : CodeActivity<string>
    {
        // Define an activity input argument of type string
        public InArgument<string> OutputFile { get; set; }
        public InArgument<string> FileName { get; set; }


        //[RequiredArgument]
        //example server path: \\development.stamats.com\BuildingsDev\_build
        public InArgument<string> ServerName { get; set; }
        public InArgument<IBuildDetail> BuildDetail { get; set; }
        public InArgument<IList<Changeset>> AssocSets { get; set; }
        public StandardKernel Kernel = new StandardKernel();

        public BuildActivity()
        { }
        protected override string Execute(CodeActivityContext context)
        {
            // Obtain the runtime value of the Text input argument
            string text = context.GetValue(this.OutputFile);
            string filename = context.GetValue(this.FileName);
            string configFilePath = context.GetValue(this.ServerName);
            //http://team:8080/tfs/WebSites

           /*
            string BaseTFSDir = bldConfig.Rules["BaseTFSDir"].Value;
            string BaseDeploymentDir = bldConfig.Rules["BaseDeploymentDir"].Value;
            string BuildDeploymentDir = bldConfig.Rules["BuildDeploymentDir"].Value;
            */

            IBuildDetail bldD = context.GetValue(BuildDetail);
            
            TfsTeamProjectCollection tfs = bldD.BuildServer.TeamProjectCollection;
            ///new  TfsTeamProjectCollection(new Uri(context.GetValue(ServerName) as string));//
            VersionControlServer vcs = (VersionControlServer)tfs.GetService(typeof(VersionControlServer));
            
            bldD.RefreshAllDetails();
            //commensurate the zip deployment with the current build
            TeamFoundationIdentity id;
            tfs.GetAuthenticatedIdentity(out id);
            //ZipDeployer.UID = (id == null)?"UnknownUser" : id.UniqueName;

            //var changesets = InformationNodeConverters.GetAssociatedChangesets(bldD);//.OrderBy(a=> a.ChangesetId);
            var changesets = context.GetValue(this.AssocSets);


            TFSDeployerController deploy = new TFSDeployerController(tfs.Uri.AbsoluteUri, configFilePath, bldD.BuildNumber);
            
            return deploy.deploy(tfs, changesets);
            /**/
            //if (string.IsNullOrEmpty(text))
            //  throw new ArgumentException("Please specify a path");
            //String.Format("{0}@$/{1}", LabelName, BuildDetail.BuildDefinition.TeamProject)
           
        }
    }
}