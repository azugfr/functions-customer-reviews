param(
    [Parameter(Mandatory=$True)]
    [string]
    $resourceGroup,

    [Parameter(Mandatory=$True)]
    [string]
    $uniqueKey
)

$storageKey=az storage account keys list -g $resourceGroup -n $uniqueKey'stor' --query [0].value
$storageConnection="DefaultEndpointsProtocol=https;AccountName=$($uniqueKey)stor;AccountKey=$($storageKey.Value);EndpointSuffix=core.windows.net"

$docdbUri="https://$($uniqueKey)docdb.documents.azure.com:443/"
$docdbKey=(az cosmosdb keys list -g $resourceGroup -n $uniqueKey'docdb' | ConvertFrom-Json)

(Get-Content $PSScriptRoot\config.xml) -replace "YOURSTORAGECONNECTIONSTRING",$storageConnection `
                                       -replace "YOURCOSMOSDBENDPOINT",$docdbUri `
                                       -replace "YOURCOSMOSDBKEY",$docdbKey.primaryMasterKey `
| Set-Content $PSScriptRoot\config.xml