using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using toDev.Concrete.ZipDeployer;

namespace Deployers
{
    public abstract class DeployerController
    {
        protected string baseDir { get; set; }
        protected string buildDir { get; set; }
        protected string buildNumber { get; set; }
        protected BuildConfiguration _configuration;
        /// <summary>
        /// Initiate a deployer controller
        /// </summary>
        /// <param name="baseDir">The base directory (start for all relative directories)</param>
        /// <param name="DeploymentDir">Full directory where build artifacts are stored</param>
        /// <param name="buildNumber">The build number</param>
        protected DeployerController(string baseDir, string BuildDir, string buildNumber)
        {
            setConfiguration(baseDir, BuildDir, buildNumber);
        }

        protected void setConfiguration(string baseDir, string BuildDir, string buildNumber)
        {

            this.baseDir = baseDir;
            this.buildDir = BuildDir;
            this.buildNumber = buildNumber;
        }
        protected DeployerController(BuildConfiguration config, string buildNumber)
        {
            setConfiguration(config, buildNumber);
        }

        protected void setConfiguration(BuildConfiguration config, string buildNumber)
        {
            _configuration = config;
            setConfiguration(_configuration.Rules["BaseDeploymentDir"].Value, _configuration.Rules["BuildDeploymentDir"].Value.TrimEnd('\\'), buildNumber);
        }
        protected DeployerController()
        {
        
        }
    }
}
