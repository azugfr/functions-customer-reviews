param(
$resourceGroup,
$uniqueKey
)

$storageKey=Get-AzureRmStorageAccountKey -ResourceGroupName $resourceGroup `
                                         -AccountName $uniqueKey'stor' `
            | Select-Object -first 1
$storageConnection="DefaultEndpointsProtocol=https;AccountName=$($uniqueKey)stor;AccountKey=$($storageKey.Value);EndpointSuffix=core.windows.net"
$docdbUri="https://$($uniqueKey)docdb.documents.azure.com:443/"
$docdbKey=Invoke-AzureRmResourceAction -Action listKeys `
              -ResourceType 'Microsoft.DocumentDb/databaseAccounts' `
              -ResourceGroupName $resourceGroup `
              -Name $uniqueKey'docdb' `
			  -Force `
          | Select-Object -first 1

(Get-Content .\config.xml) -replace "YOURSTORAGECONNECTIONSTRING",$storageConnection `
                           -replace "YOURCOSMOSDBENDPOINT",$docdbUri `
                           -replace "YOURCOSMOSDBKEY",$docdbKey.primaryMasterKey `
| Set-Content .\config.xml