using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2;
using Autofac;
using AWS.Application;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AWS.Infrastructure
{
    public class AWSServiceModule : Autofac.Module
    {
        private readonly IConfiguration _configuration;

        public AWSServiceModule(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public AWSServiceModule()
        {

        }
        protected override void Load(ContainerBuilder builder)
        {
          
            var dynamoDBClient = new AmazonDynamoDBClient();
            builder.RegisterInstance(dynamoDBClient)
                   .As<IAmazonDynamoDB>();
            builder.RegisterType<SQSMessageService>().As<ISQSMessageService>();
            builder.RegisterType<S3FileService>().As<IS3FileService>();
            builder.RegisterType<DynamoDbService>().As<IDynamoDbService>();
            
        }
    }
}
