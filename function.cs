using Amazon.Lambda.Core;
using Alexa.NET.Response;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Newtonsoft.Json;
using Alexa.NET.APL;
using Alexa.NET.Response.APL;
using Alexa.NET.APL.Components;
// See https://github.com/stoiveyp/Alexa.NET.APL

// Assembly attribute to enable the Lambda function's JSON input
// to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
namespace AWSLambda1
{
  public class Function
  {
    public SkillResponse FunctionHandler(SkillRequest input, ILambdaContext context)
    {
      APLSupport.Add(); 
      RenderDocumentDirective.AddSupport();
      SkillResponse response = new SkillResponse();
      response.Response = new ResponseBody();
      response.Response.ShouldEndSession = false;
      IOutputSpeech innerResponse = null;
      var log = context.Logger;
      log.LogLine($"Skill Request Object:");
      log.LogLine(JsonConvert.SerializeObject(input));
      if (input.GetRequestType() == typeof(LaunchRequest))
      {
        log.LogLine($"Default LaunchRequest made");
        innerResponse = new PlainTextOutputSpeech();
        (innerResponse as PlainTextOutputSpeech).Text = "Launch Request";
      }
      else if (input.GetRequestType() == typeof(IntentRequest))
      {
        var intentRequest = (IntentRequest)input.Request;
        switch (intentRequest.Intent.Name)
        {
          case "AMAZON.CancelIntent":
            log.LogLine($"AMAZON.CancelIntent: send StopMessage");
            innerResponse = new PlainTextOutputSpeech();
            (innerResponse as PlainTextOutputSpeech).Text = "Cancelled";
            response.Response.ShouldEndSession = true;
            break;
          case "AMAZON.StopIntent":
            log.LogLine($"AMAZON.StopIntent: send StopMessage");
            innerResponse = new PlainTextOutputSpeech();
            (innerResponse as PlainTextOutputSpeech).Text = "Stopping";
            response.Response.ShouldEndSession = true;
            break;
          case "AMAZON.HelpIntent":
            log.LogLine($"AMAZON.HelpIntent: send HelpMessage");
            innerResponse = new PlainTextOutputSpeech();
            (innerResponse as PlainTextOutputSpeech).Text = "Help me!";
            break;
          case "HelloIntent":
            log.LogLine($"Say Hello");

            var directive = new RenderDocumentDirective
            {
              Token = "randomToken",
              Document = new APLDocument		// in Alexa.NET.Response.APL
              {
                MainTemplate = new Layout(new[]
                {
                  new Container(new APLComponent[]{
                    new Text("APL in C#"){FontSize = "24dp",TextAlign= "Center"},
                    new Image("https://johnallworksbucket.s3.amazonaws.com/perkin.jpg"){Width = 600,Height=480}
                  })
                })
              }
            };

            response.Response.Directives.Add(directive);

            innerResponse = new PlainTextOutputSpeech();
            (innerResponse as PlainTextOutputSpeech).Text = "Hello Intent";
            break;
          default:
            log.LogLine($"Unknown intent: " + intentRequest.Intent.Name);
            innerResponse = new PlainTextOutputSpeech();
            (innerResponse as PlainTextOutputSpeech).Text = "Unknown Intent";
            break;
        }
      }
      response.Response.OutputSpeech = innerResponse;
      response.Version = "1.0";
      log.LogLine($"Skill Response Object...");
      log.LogLine(JsonConvert.SerializeObject(response));
      return response;
    }
  }
}
