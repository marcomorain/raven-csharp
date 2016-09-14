<%@ Page Language="C#" Async="true" %>
<%@ Import Namespace="SharpRaven" %>
<%@ Import Namespace="System.Threading.Tasks" %>

<!DOCTYPE html>
<script runat="server">

    private const string dsnUrl = "https://cd95d48687a54ee1840a16ecef394c93:c9def31f1d5940b18b2a9b4ba149b19d@sentry.io/75499";


    private static async Task<int> Recurser(int stackframes) {
        return await DivideByZero(stackframes);
    }

    private async static Task<int> DivideByZero(int stackFrames = 10)
    {
        if (stackFrames == 0)
        {
            var a = 0;
            return 1 / a;
        }
        else {
            return await Recurser(--stackFrames);
        } 
    }


    private void Page_Load(object sender, EventArgs e)
    {
        Title = "Capture exception";

        if (Request.HttpMethod != "POST")
            return;

        Title = "Exception captured!";

        var client = new RavenClient(dsnUrl);

        try
        {
            Task<int> task = DivideByZero();
            Task.WaitAll(task);
        }
        catch (Exception exception)
        {
            client.CaptureException(exception);
        }
    }


</script>
<html>
    <head id="Head1" runat="server">
        <title><%= Title %></title>
    </head>
    <body>
        <h1><%= Title %></h1>
        <form method="post">
            <input type="hidden" name="Hidden" value="I'm hidden!">
            <input type="submit" name="Button" value="Capture!">
        </form>
    </body>
</html>