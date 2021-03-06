﻿using Nancy.Metadata.Modules;
using Nancy.Metadata.Swagger.Core;
using Nancy.Metadata.Swagger.DemoApplication.Model;
using Nancy.Metadata.Swagger.Fluent;
using Nancy.ModelBinding;
using System;

namespace Nancy.Metadata.Swagger.DemoApplication.Modules
{
    public class RootModule : NancyModule
    {
        public RootModule() : base("/api")
        {
            Get("/hello", r => HelloWorld(), name: "SimpleRequest");
            Get("/hello/{name}", r => Hello(r.name), name: "SimpleRequestWithParameter");
            Post("/hello", r => HelloPost(), name: "SimplePostRequest");
            Post("hello/model", r => HelloModel(), name: "PostRequestWithModel");
            Post("/hello/nestedmodel", r => HelloNestedModel(), name: "PostRequestWithNestedModel");
        }

        private Response HelloNestedModel()
        {
            try
            {
                NestedRequestModel model = this.BindAndValidate<NestedRequestModel>();


                if (!ModelValidationResult.IsValid)
                {
                    return Response
                         .AsJson(new ValidationFailedResponseModel(ModelValidationResult))
                         .WithStatusCode(HttpStatusCode.BadRequest);
                }

                var response = new SimpleResponseModel
                {
                    Hello = $"Hello, {model?.SimpleModel.Name}. We got your name from a nested object."
                };

                return Response.AsJson(response);
            }

            catch (ModelBindingException)
            {
                return Response
                     .AsJson(new ValidationFailedResponseModel("Model Binding Failed with Exception."))
                     .WithStatusCode(HttpStatusCode.BadRequest);
            }

            catch (NullReferenceException)
            {
                return Response
                .AsJson(new ValidationFailedResponseModel("The body contains an invalid nested request model."))
                .WithStatusCode(HttpStatusCode.InternalServerError);
            }
        }

        private Response HelloModel()
        {
            try
            {
                SimpleRequestModel model = this.BindAndValidate<SimpleRequestModel>();

                if (!ModelValidationResult.IsValid)
                {
                    return Response
                         .AsJson(new ValidationFailedResponseModel(ModelValidationResult))
                         .WithStatusCode(HttpStatusCode.BadRequest);
                }

                var response = new SimpleResponseModel
                {
                    Hello = $"Hello, {model.Name}."
                };

                return Response.AsJson(response);
            }
            catch (ModelBindingException)
            {
                return Response
                     .AsJson(new ValidationFailedResponseModel("Model Binding Failed with Exception."))
                     .WithStatusCode(HttpStatusCode.BadRequest);
            }

            catch (Exception ex)
            {
                return Response
                .AsJson(new ValidationFailedResponseModel(ex.Message))
                .WithStatusCode(HttpStatusCode.InternalServerError);
            }

        }

        private Response HelloPost()
        {
            var response = new SimpleResponseModel
            {
                Hello = "Hello Post!"
            };

            return Response.AsJson(response);
        }

        private Response Hello(string name)
        {
            var response = new SimpleResponseModel
            {
                Hello = $"Hello, {name}."
            };

            return Response.AsJson(response);
        }

        private Response HelloWorld()
        {
            var response = new SimpleResponseModel
            {
                Hello = "Hello World!"
            };

            return Response.AsJson(response);
        }
    }

    public class RootMetadataModule : MetadataModule<SwaggerRouteMetadata>
    {
        public RootMetadataModule()
        {
            Describe["SimpleRequest"] = desc => new SwaggerRouteMetadata(desc)
                .With(i => i.WithResponseModel("200", typeof(SimpleResponseModel), "Sample response")
                            .WithSummary("Simple GET example"));

            Describe["SimpleRequestWithParameter"] = desc => new SwaggerRouteMetadata(desc)
                .With(i => i.WithResponseModel("200", typeof(SimpleResponseModel), "Sample response")
                            .WithRequestParameter("name")
                            .WithSummary("Simple GET with parameters"));

            Describe["SimplePostRequest"] = desc => new SwaggerRouteMetadata(desc)
                .With(info => info.WithResponseModel("200", typeof(SimpleResponseModel), "Sample response")
                    .WithSummary("Simple POST example"));

            Describe["PostRequestWithModel"] = desc => new SwaggerRouteMetadata(desc)
                .With(info => info.WithResponseModel("200", typeof(SimpleResponseModel))
                    .WithResponseModel("400", typeof(ValidationFailedResponseModel))
                    .WithSummary("Simple POST example with request model")
                    .WithRequestModel(typeof(SimpleRequestModel)));

            Describe["PostRequestWithNestedModel"] = desc => new SwaggerRouteMetadata(desc)
                    .With(info => info.WithResponseModel("200", typeof(SimpleResponseModel))
                    .WithResponseModel("400", typeof(ValidationFailedResponseModel))
                    .WithSummary("Simple POST example with nested request model")
                    .WithRequestModel(typeof(NestedRequestModel)));
        }
    }
}
