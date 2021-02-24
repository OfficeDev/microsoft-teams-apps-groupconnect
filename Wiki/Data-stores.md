The app uses the following data stores:
1. Azure Storage Account
2. Application Insights

All these resources are created in your Azure subscription. None are hosted directly by Microsoft.

## Azure Table Storage

### Teams Data

This table stores the corresponding team information for a Diversity and Inclusion group which is registered in the app.

| Value         | Description
| ---           | ---
| PartitionKey  | Constant value as 'Team Data'.
| RowKey        | The team Id in teams.
| Timestamp     | The latest DateTime record.
| Name          | The team's name.
| ServiceUrl    | The service URL that can be used to fetch the team's roster.
| TeamId        | The team Id in teams.
| TenantId      | The teams's tenant identifier.

### Users Collection

This table stores all the users information who have installed the personal app.

| Value         | Description
| ---           | ---
| PartitionKey  | Constant value as 'User Data'.
| RowKey        | The team Id in teams.
| Timestamp     | The latest DateTime record.
| Name         | The team's name.
| ServiceUrl    | The service URL that can be used to fetch the team's roster.
| UserId      | The user's Id in teams
| TenantId        | The teams's tenant identifier.

### AppConfig Collection

The App Config Collection stores the user app Id, service url and knowledgebase id.

| Value         | Description
| ---           | ---
| PartitionKey  | Constant value as 'Settings'.
| RowKey        | Constants as "ServiceUrl" or "UserAppId". "ServiceUrl" - The value stored is service url. "UserAppId" - The value stored is user app Id. “knowledgeBaseId” – The FAQ knowledge base Id.
| Timestamp     | The latest DateTime record.
| Name         | The team's name

### Notification Data

The Notification Collection stores the notification data.

| Value             | Description
| ---               | ---
| PartitionKey      | Constants as "DraftNotifications" or "SendingNotifications" or "SentNotifications" or "GlobalSendingNotificationData". "DraftNotifications" - The notification is stored in this partition when it is in draft state. "SendingNotifications" - This partition stores the notifcation entry that is used for sending the notification and serialized JSON content. "SentNotifications" - The notification is moved to this partition when it is sent to the recipient. "GlobalSendingNotificationData" - This partition stores the Retry Delay time when the system is being throttled.
| RowKey            | The notification unique identifier.
| Timestamp         | The latest DateTime record.
| Id                | The notification identifier.
| Title             | The title text of notification's content.
| ImageLink         | The image link of notification's content.
| Summary           | The summary text of notification's content.
| Author            | The author text of the notification's content.
| ButtonLink        | The button link of the notification's content.
| ButtonTitle       | The button title of the notification's content.
| TotalMessageCount | The total number of messages to send.
| Succeeded         | The number of recipients who have received the notification succesfully.
| Failed            | The number of recipients who failed to receive the notification succesfully.
| AllUsers          | Indicating if the notification should be sent to all known users.
| TeamsInString     | The list of team identifiers.
| RostersInString   | The list of roster identifiers.
| GroupsInString    | The list of group identifiers.
| IsCompleted       | [Deprecated] Indicating if the notification sending process is completed.
| IsDraft           | Indicating if the notifcation is a draft.
| IsPreparingToSend | [Deprecated] Indicating if the notification is in the "preparing to send" state.
| Unknown           | The number of recipients who have an unknown status.
| Content           | The content of the notification in serialized JSON form.
| NotificationId    | The notification identifier.
| RecipientNotFound | The number of not found recipients.
| CreatedBy         | The user that created the notification.
| CreatedDate       | The DateTime when notification was created.
| SendingStartDate  | The DateTime when the notification sending was started.
| SentDate          | The DateTime when the notification's sending was completed.
| WarningMessage    | The warning message for the notification if there was a warning given when preparing and sending notification.
| ErrorMessage      | The error message for the notification if there was a failure in preparing and sending notification.
| Status            | The notification status.

### SentNotification Data

The SentNotification Collection stores the sent notification data.

| Value             | Description
| ---               | ---
| PartitionKey      | The notification unique identifier.
| RowKey            | The user's identifier.
| Timestamp         | The latest DateTime record.
| ConversationId    | The conversation identifier for the recipient.
| IsStatusCodeFromCreateConversation| Indicating if the status code is from the create conversation call.
| NumberOfFunctionAttemptsToSend    | The number of times an Azure Function instance attempted to send the notification to the recipient.
| RecipientId       | The recipients unique identifier.
| RecipientType     | Indicating which type of recipient the notification was sent.
| SentDate          | The DateTime when the notification's sending was completed.
| ServiceUrl        | The service URL of the recipient.
| StatusCode        | The status code for the notification received by the bot.
| TenantId          | The tenant identifier of the recipient.
| TotalNumberOfSendThrottles        | The total number of throttle responses the bot received when trying to send the notification to the recipient.
| UserId            | The user identifier of the recipient.

### Export Data

The Export Collection stores the export data.

| Value             | Description
| ---               | ---
| PartitionKey      | The user's azure active directory identifier.
| RowKey            | The notification identifier.
| Timestamp         | The latest DateTime record.
| SendDate          | The export send date.
| Status            | The file export status.

### Employee Resource Group

This table stores the information for the Employee Resource Groups created by end-users.

| Value             | Description
| ---               | ---
| Partition Key (Group Type)      | This represents the partition key of the azure storage table - Type of the group either External or Teams.
| RowKey            | Represents the unique group id of each row.
| Timestamp         | The latest DateTime record.
| GroupId           | Same as Row key.
| GroupName         | Represents the employee resource group name or team name.
| GroupDescription  | Represents the employee resource group description or team description.
| GroupLink         | Represents the external link or team link.
| ImageLink         | Represents the image link.
| Tags              | Semicolon separated tags added by user.
| Location          | Represents the location of the user.
| IncludeInSearchResults | True if search wants to be enabled not false. Only search enabled groups are available to all end users.
| IsDeleted         | True if group moved to deleted state not false. It will not be permanently deleted from storage instead a flag will set as deleted. Deleted group members will not be considered for any pair-up meetings.
| ApprovalStatus    | Admin team member can Approve/Reject any ERG request to make it searchable. InProgress=0, Approve =1, Rejected=2.
| MatchingFrequency | Pair up matching frequency for each team. It can be updated/modified by Admin team or respective team owners Weekly = 0 ,Monthly = 1.
| CreatedOn         | Contains the date time of Group creation.
| Created By UserPrincipalName   | The email address of the end user who created group..
| Created By ObjectId    | The AAD Object Id of user who created group
| Updated On        | Last date time on which group was updated.
| Updated By ObjectId           | The AAD Object Id of user who created group.

### Pair-up Mapping Data

The table stores all the pair up mapping between users and Teams.

| Value         | Description
| ---           | ---
| PartitionKey  | Team Id (19:xxx).
| RowKey        | User object identifier.
| Timestamp     | The latest DateTime record.
| TeamId        | TeamID is the unique identifier related to the specific team of which the user is a part of.
| IsPaused      | Flag set by the user to be picked for next pair up matching for that team.
| UserId        | This is the unique identifier of a user.
| TenantId      | Microsoft O365 Tenant ID.
