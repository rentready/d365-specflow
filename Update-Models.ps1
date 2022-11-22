param(
    [String]$Url = '', 
    [String]$DestDir = "src/RR.Dynamics365.Model/"
    )

function Merge-Setting {
    param (
        $xml,
        $key,
        $value
    )

    if ($null -eq $xml.configuration.appSettings) {
        $appSettings = $xml.CreateElement("appSettings")
        $xml.configuration.AppendChild($appSettings)
    }
    
    $added = $false
    foreach( $dest_add in $xml.configuration["appSettings"].add)
    {
        if ($key -eq $dest_add.Key)
        {
            if ($value -ne $dest_add.Value)
            {
                Write-Output "Found matching environment variable for key: $($key)"
                Write-Output "Replacing value $($dest_add.value)  with $value"
                $dest_add.SetAttribute("value", $value)
            }

            $added = $true
        }
    }

    if (!$added) {
        $newNode = $xml.CreateElement("add")
        $newNode.SetAttribute("key", $key)
        $newNode.SetAttribute("value", $value)
        $xml.configuration["appSettings"].AppendChild($newNode)
    }
}

$scriptDir = $MyInvocation.MyCommand.Path | Split-Path -Parent
Set-Location -Path $scriptDir

Write-Host "Model files generating. It will take a few minutes."

$modelsPath = Join-Path $scriptDir $DestDir

nuget install DLaB.Xrm.EarlyBoundGenerator -Version 1.2022.9.22 -O .\Tools

$entitiesJsonPath = Join-Path $scriptDir "early-bound-entities.json"
$entities = Get-Content $entitiesJsonPath | Out-String | ConvertFrom-Json

$entitiesWhiteListValue = $entities -Join '|'
$configFileName = 'CrmSvcUtil.exe.config'
$sourceConfigPath = Join-Path $scriptDir 'EarlyBoundGenerator.config'
$destConfigPath = (Get-ChildItem -Path (Join-Path $scriptDir "Tools") -Recurse -Include $configFileName).FullName
$sourceXml = (Get-Content $sourceConfigPath) -as [Xml]
$destXml = (Get-Content $destConfigPath) -as [Xml]

# Replace ApplicationSetting Keys from Prod to installed
Merge-Setting -xml $destXml -key 'EntitiesWhitelist' -value $entitiesWhiteListValue
foreach( $source_add in $sourceXml.configuration.appSettings.add)
{
    Merge-Setting -xml $destXml -key $source_add.Key -value $source_add.Value
}

# Save the installed Config
$destXml.Save($destConfigPath)

Write-Host "Config file has been updated."

$crmsvcutil = (Get-ChildItem -Path (Join-Path $scriptDir "Tools") -Recurse -Include 'crmsvcutil.exe')
Set-Location -Path $crmsvcutil.Directory

.\crmsvcutil.exe /url:$URL /namespace:"RR.Dynamics365.Model" /SuppressGeneratedCodeAttribute /out:$(Join-Path $modelsPath CrmServiceContext.cs) /servicecontextname:CrmServiceContext /codecustomization:DLaB.CrmSvcUtilExtensions.Entity.CustomizeCodeDomService,DLaB.CrmSvcUtilExtensions /codegenerationservice:DLaB.CrmSvcUtilExtensions.Entity.CustomCodeGenerationService,DLaB.CrmSvcUtilExtensions /codewriterfilter:DLaB.CrmSvcUtilExtensions.Entity.CodeWriterFilterService,DLaB.CrmSvcUtilExtensions /namingservice:DLaB.CrmSvcUtilExtensions.NamingService,DLaB.CrmSvcUtilExtensions /metadataproviderservice:DLaB.CrmSvcUtilExtensions.Entity.MetadataProviderService,DLaB.CrmSvcUtilExtensions /username:"" /password:"*********" /interactivelogin:true 
.\crmsvcutil.exe /url:$URL /namespace:"RR.Dynamics365.Model" /SuppressGeneratedCodeAttribute /out:$(Join-Path $modelsPath OptionSets.cs) /codecustomization:DLaB.CrmSvcUtilExtensions.OptionSet.CustomizeCodeDomService,DLaB.CrmSvcUtilExtensions /codegenerationservice:DLaB.CrmSvcUtilExtensions.OptionSet.CustomCodeGenerationService,DLaB.CrmSvcUtilExtensions /codewriterfilter:DLaB.CrmSvcUtilExtensions.OptionSet.CodeWriterFilterService,DLaB.CrmSvcUtilExtensions /namingservice:DLaB.CrmSvcUtilExtensions.NamingService,DLaB.CrmSvcUtilExtensions /metadataproviderservice:DLaB.CrmSvcUtilExtensions.BaseMetadataProviderService,DLaB.CrmSvcUtilExtensions /username:"" /password:"*********" /interactivelogin:true 

git add "$($modelsPath)*"