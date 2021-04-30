# azure-function-json-transformer
Using Just.Net capabilities to provide json transformation as an Azure Function with Http Trigger endpoint

This function http trigger will accept a json as request body and will respond and transformed json by doing transformation against a mapping file that you specify in the url routing. The mapping file needs to exist in the Azure storage container "mapping-files" as specified in the code. The Function will use a Storage Emulator when running locally but a real Storage Account is required when deployed to Azure.

# Request Exampe
```
curl --location --request POST 'http://localhost:7071/api/transform/mapping-test' \
--header 'Content-Type: application/json' \
--data-raw '{
  "menu": {
    "popup": {
      "menuitem": [{
          "value": "Open",
          "onclick": "OpenDoc()"
        }, {
          "value": "Close",
          "onclick": "CloseDoc()"
        }
      ],
	  "submenuitem": "CloseSession()"
    }
  }
}'
```

# Mapping file example in storage container mapping-files

## mapping-test.json

```
{
  "result": {
    "Open": "#valueof($.menu.popup.menuitem[?(@.value=='Open')].onclick)",
    "Close": "#valueof($.menu.popup.menuitem[?(@.value=='Close')].onclick)"
  }
}
```


# Response

```
{
    "result": {
        "Open": "OpenDoc()",
        "Close": "CloseDoc()"
    }
}
```
