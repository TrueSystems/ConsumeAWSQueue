# ConsumeAWSQueue

Simple command-line application to consume a AWS SQS queue and save message body to file.

```
Usage: ConsumeAWSQueue -a awsAccessKey - s awsSecretKey -q awsSQSQueueUrl [-u awsSQSEndpoint]
```
###Exit Codes:  

&nbsp;&nbsp;&nbsp;&nbsp;0 - Executed Successfully  
 
&nbsp;&nbsp;&nbsp;&nbsp; Other - Error

###Parameters:
 
&nbsp;&nbsp;&nbsp;&nbsp; -a --> Your AWS Access Key (never share it !) [required]  
&nbsp;&nbsp;&nbsp;&nbsp; -s --> Your AWS Secret Key (never share it !) [required]  
&nbsp;&nbsp;&nbsp;&nbsp; -q --> AWS SQS Queue URL [required]  
&nbsp;&nbsp;&nbsp;&nbsp; -u --> AWS SQS Endpoint [optional] Defaults to sa-east-1
 
 
 



