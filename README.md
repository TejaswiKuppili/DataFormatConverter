# DataFormatConverter
**1.	Problem Statement**
Insurance companies receive claims data from multiple sources:
•	Hospitals -> XML (HL7, custom XML schemas).
•	Agents/Portals -> JSON (modern REST APIs).
•	Legacy batch uploads -> CSV (spreadsheets).
Core insurance system (Policy Admin / Claims Processing) might only accept JSON or a specific XML schema.
Without a converter, every integration requires custom, hard-coded transformations, which increases cost and slows down onboarding.
We needed a way to convert data between different formats (JSON, XML, CSV, Canonical) dynamically through an API. Instead of writing multiple one-off converters, I built a pluggable Data Format Converter service using Azure Functions and exposed it securely via API Management (APIM).

**2.	Architecture Overview**
•	Azure Function App → hosts the conversion logic.
•	ConversionService (core logic) → takes input format, output format, and data.
•	Format Handlers (strategy pattern):
o	JsonHandler
o	XmlHandler
o	CsvHandler
o	CanonicalHandler
Each handler implements a common IFormatHandler interface with Serialize and Deserialize.
•	Repository → resolves the correct handler at runtime based on the requested format.
•	API Management (APIM) → secures and exposes the Function App endpoint to clients with policies (auth, rate-limiting, validation).

**Test Cases**
Scenario 1: JSON → XML
{
  "inputFormat": "json",
  "outputFormat": "xml",
  "data": "{\"name\":\"Tejaswi\",\"age\":24}"
}
Scenario 2: JSON → CSV
{
  "inputFormat": "json",
  "outputFormat": "csv",
  "data": "[{\"name\":\"Tejaswi\",\"age\":24},{\"name\":\"John\",\"age\":25}]"
}
Scenario 3: CSV → JSON
{
  "inputFormat": "csv",
  "outputFormat": "json",
  "data": "name,age\nTejaswi,24\nJohn,25"
}
Scenario 4: CSV → XML
{
  "inputFormat": "csv",
  "outputFormat": "xml",
  "data": "name,age\nTejaswi,24\nJohn,25"
}


Scenario 5: XML → JSON
{
  "inputFormat": "xml",
  "outputFormat": "json",
  "data": "<Root><name>Tejaswi</name><age>24</age></Root>"
}
Scenario 6: XML → CSV
{
  "inputFormat": "xml",
  "outputFormat": "csv",
  "data": "<Root><name>Tejaswi</name><age>24</age></Root>"
}

**Edge Cases / Invalid Requests**
Scenario 7: Null/Empty Fields
{
  "inputFormat": "",
  "outputFormat": "json",
  "data": null
}
Scenario 8: Unsupported Format
{
  "inputFormat": "yaml",
  "outputFormat": "json",
  "data": "some data"
}

Scenario 9: Invalid JSON
{
  "inputFormat": "json",
  "outputFormat": "xml",
  "data": "{name:Tejaswi, age:24}"
}

Scenario 10: Single Value Conversion
{
  "inputFormat": "json",
  "outputFormat": "xml",
  "data": "\"Hello World\""
}
Scenario 11: Nested JSON
{
  "inputFormat": "json",
  "outputFormat": "xml",
  "data": "{\"person\":{\"name\":\"Tejaswi\",\"age\":30},\"city\":\"Bangalore\"}"
}
Scenario 12: JSON → Canonical
{
  "inputFormat": "json",
  "outputFormat": "canonical",
  "data": "{\"PolicyId\":\"P123\",\"CustomerName\":\"Tejaswi\",\"ClaimAmount\":5000}"
}
Scenario 13: Canonical → JSON
{
  "inputFormat": "canonical",
  "outputFormat": "json",
  "data": "PolicyId=P123|CustomerName=Tejaswi|ClaimAmount=5000"
}
Scenario 14: Canonical → XML
{
  "inputFormat": "canonical",
  "outputFormat": "xml",
  "data": "PolicyId=P123|CustomerName=Tejaswi|ClaimAmount=5000"
}
Scenario 15: Canonical → CSV
{
  "inputFormat": "canonical",
  "outputFormat": "csv",
  "data": "PolicyId=P123|CustomerName=Tejaswi|ClaimAmount=5000"
}
