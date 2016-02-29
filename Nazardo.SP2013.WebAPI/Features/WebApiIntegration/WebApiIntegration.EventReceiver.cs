using System;
using System.Runtime.InteropServices;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Nazardo.SP2013.WebAPI.Integration;

namespace Nazardo.SP2013.WebAPI.Features.WebApiIntegration
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("521e363f-827d-4579-9f66-58c9b91b4a1a")]
    public class WebApiIntegrationEventReceiver : SPFeatureReceiver
    {
        /// <summary>
        /// Handles the event raised after a feature has been activated.
        /// Installs the WebApiModule as WebConfigModification
        /// </summary>
        /// <param name="properties"></param>
        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            SPDiagnosticsService.Local.WriteTrace(0,
                                                  new SPDiagnosticsCategory(
                                                      "Nazardo.SP2013.WebAPI",
                                                      TraceSeverity.Medium,
                                                      EventSeverity.Information),
                                                  TraceSeverity.Medium,
                                                  string.Format(
                                                      "Feature: Registering HTTPModule for WebApi"),
                                                  null);
            RegisterHttpModule(properties);
        }

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            SPDiagnosticsService.Local.WriteTrace(0,
                                                  new SPDiagnosticsCategory(
                                                      "Nazardo.SP2013.WebAPI",
                                                      TraceSeverity.Medium,
                                                      EventSeverity.Information),
                                                  TraceSeverity.Medium,
                                                  string.Format("Feature: Removing HTTPModule for WebApi"),
                                                  null);
            UnregisterHttpModule(properties);
        }

        private void RegisterHttpModule(SPFeatureReceiverProperties properties)
        {
            SPWebConfigModification webConfigModification = CreateWebModificationObject();

            SPSecurity.RunWithElevatedPrivileges(() =>
            {
                SPWebService contentService = SPWebService.ContentService;

                contentService.WebConfigModifications.Add(webConfigModification);
                contentService.Update();
                contentService.ApplyWebConfigModifications();
            });
        }

        /// <summary>
        /// Create the SPWebConfigModification object for the signalr module
        /// </summary>
        /// <returns>SPWebConfigModification object for the http module to the web.config</returns>
        private SPWebConfigModification CreateWebModificationObject()
        {
            string name = string.Format("add[@name=\"{0}\"]", typeof(WebApiModule).Name);
            string xpath = "/configuration/system.webServer/modules";

            SPWebConfigModification webConfigModification = new SPWebConfigModification(name, xpath);

            webConfigModification.Owner = "Nazardo.SP2013.WebAPI";
            webConfigModification.Sequence = 0;
            webConfigModification.Type = SPWebConfigModification.SPWebConfigModificationType.EnsureChildNode;

            //reflection safe
            webConfigModification.Value = String.Format("<add name=\"{0}\" type=\"{1}\" />", typeof(WebApiModule).Name,
                                                        typeof(WebApiModule).AssemblyQualifiedName);
            return webConfigModification;
        }

        private void UnregisterHttpModule(SPFeatureReceiverProperties properties)
        {

            SPWebConfigModification webConfigModification = CreateWebModificationObject();

            SPSecurity.RunWithElevatedPrivileges(() =>
            {
                SPWebService contentService = properties.Definition.Farm.Services.GetValue<SPWebService>();

                int numberOfModifications = contentService.WebConfigModifications.Count;

                //Iterate over all WebConfigModification and delete only those we have created
                for (int i = numberOfModifications - 1; i >= 0; i--)
                {
                    SPWebConfigModification currentModifiction = contentService.WebConfigModifications[i];

                    if (currentModifiction.Owner.Equals(webConfigModification.Owner))
                    {
                        contentService.WebConfigModifications.Remove(currentModifiction);
                    }
                }

                //Update only if we have something deleted
                if (numberOfModifications > contentService.WebConfigModifications.Count)
                {
                    contentService.Update();
                    contentService.ApplyWebConfigModifications();
                }

            });
        }
    }
}
