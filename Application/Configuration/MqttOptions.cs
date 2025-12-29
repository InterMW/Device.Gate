namespace Application.Configuration;

public class MqttOptions
{
   public static string Section => "Mqtt";

   public string User { get; set; } = "";
   public string Password { get; set; } = "";
   public string Host { get; set; } = "";
}
