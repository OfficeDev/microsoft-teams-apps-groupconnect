## Assumptions

The estimate below assumes:
* 1000 users in the tenant
* 1 message sent to all users each week (~5/month)
* Administrator opts to create a custom domain name and obtain an SSL certificate for the site. 
    * When purchased through Azure, this is *typically* ~$12 for a domain name, and $75/year for the SSL certificate.
    * If you choose to use Azure Front Door, that adds a monthly cost of $46 (for 2 routing rules + minimal bandwidth consumption).

> The template defaults to using Azure Front Door, to reduce the cost of trying out and experimenting with the template, rather than requiring you to get a custom domain outright. For instance, you can run your instance for a few days, then turn off the services, and only pay for your actual Azure usage when the services were on.

We ignore:
* Operations associated with app installations, as that happens only once per user/team
* Operations associated with the authors viewing the tab, given the assumption that they are sending only 5 messages/month.
* Executions associated with Prep or Send function, as the execution count is trivial to the calculations.

## SKU recommendations

The recommended SKUs for a production environment are:
* App Service: Standard (S1)
* QnAMaker: Standard (S0)
* Azure Search: Basic
    -   Create up to 14 knowledge bases
    -   The Azure Search service cannot be upgraded once it is provisioned, so select a tier that will meet your anticipated needs.
* Service Bus: Basic

## Estimated load

**Number of messages sent**: 1000 users * 5 messages/month = 5000 messages

**Number of QnA queries**: 500 users * 5 questions/user/day * 30 (number of days in a month) = 75000 questions/month

**Data storage**: 1 GB max    
* Messages are on the order of KBs

**Table data operations**:

* Prep function
    * (3 read * 5000 messages) = 15000 reads

* Send function
    * (4 read * 5000 messages) = 20000 reads
    * (1 write * 5000 messages) = 5000 writes

* Data function
    * Aggregating status: (1 read * 5000 messages) = 5000 reads

**Service bus operations**:
* ((1 write + 1 read) operations/sent message * 5000 messages) = 10000 operations

**Azure Function**:
> For Gb-sec pricing calculation, please refer this formula.
(Memory Size * Execution Time * Execution Count)/1024000.
Min. memory size = 128 Mb. 
Min. execution time = 100 ms.

* Send function
    * (1 invocation * 5000 messages) = 5000 invocations
    * (128 Mb * 3000 * 5000 messages)/1024000 = 1875 Gb-sec

## Estimated cost

**IMPORTANT:** This is only an estimate, based on the assumptions above. Your actual costs may vary.

Prices were taken from the [Azure Pricing Overview](https://azure.microsoft.com/en-us/pricing/) on 19 October 2020, for the West US 2 region.

Use the [Azure Pricing Calculator](https://azure.com/e/c3bb51eeb3284a399ac2e9034883fcfa) to model different service tiers and usage patterns.

Resource                                    | Tier          | Load              | Monthly price
---                                         | ---           | ---               | --- 
Storage account (Table)                     | Standard_LRS  | < 1GB data, 45000 operations | $0.045 + $0.01 = $0.05
Bot Channels Registration                   | F0            | N/A               | Free
App Service Plan                            | S1            | 744 hours         | $74.40
App Service (Bot + Tab)                     | -             |                   | (charged to App Service Plan) 
Azure Function                              | Dedicated     | 10000 executions  | (free up to 1 million executions)
QnAMaker Cognitive Service                  | S0            |                   | $10
Azure Search                                | B             |                   | $75.14
Service Bus                                 | Basic         | 10000 operations  | $0.05
Application Insights                        | -             | < 5GB data        | (free up to 5 GB)
**Total**                                   |               |                   | **$159.64**


## Estimated load - 1M messages

**Data storage**: 3 GB max    
* Messages are on the order of KBs

**Number of messages sent**: 1M messages

**Table data operations**:

* Prep function
    * (3 read/prep * 1M messages) = 3M reads

* Send function
    * (4 read * 1M messages) = 4M reads
    * (1 write * 1M messages) = 1M writes

* Data function
    * Aggregating status: (1 read * 1M messages) = 1M reads

**Service bus operations**:
* ((1 write + 1 read) operations/sent message * 1M messages) = 2M operations

**Azure Function**:
> For Gb-sec pricing calculation, please refer this formula.
(Memory Size * Execution Time * Execution Count)/1024000.
Min. memory size = 128 Mb. 
Min. execution time = 100 ms.

* Send function
    * (1 invocation * 1M messages) = 1M invocations
    * (128 Mb * 3000 * 1M messages)/1024000 = 375000 Gb-sec

## Estimated cost - 1M messages

**IMPORTANT:** This is only an estimate, based on the assumptions above. Your actual costs may vary.

Prices were taken from the [Azure Pricing Overview](https://azure.microsoft.com/en-us/pricing/) on 19 October 2020, for the West US 2 region.

Use the [Azure Pricing Calculator](https://azure.com/e/c3bb51eeb3284a399ac2e9034883fcfa) to model different service tiers and usage patterns.

Resource                                    | Tier          | Load              | Monthly price
---                                         | ---           | ---               | --- 
Storage account (Table)                     | Standard_LRS  | < 1GB data, 45000 operations | $0.045 + $0.01 = $0.05
Bot Channels Registration                   | F0            | N/A               | Free
App Service Plan                            | S1            | 744 hours         | $74.40
App Service (Bot + Tab)                     | -             |                   | (charged to App Service Plan) 
Azure Function                              | Dedicated     | 10000 executions  | (free up to 1 million executions)
QnAMaker Cognitive Service                  | S0            |                   | $10
Azure Search                                | B             |                   | $75.14
Service Bus                                 | Basic         | 10000 operations  | $0.05
Application Insights                        | -             | < 5GB data        | (free up to 5 GB)
**Total**                                   |               |                   | **$159.64**