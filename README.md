# SignalRBoatChatModule


SignalR is a web-based real-time bidirectional communication framework. It’s very useful in terms of user-friendliness. While developing a chat application or broadcasting the message like notification popup, before SignalR came into the picture, we used to use the request/response technology where the server had to wait for the client’s request. On the other hand, in SignalR, we can directly push the message to the clients.

> At the end, you'll have a working chat app:

![FVCproductions](https://drive.google.com/uc?export=view&id=1RiV5xbOKxRcOeVOjkPu5VYZlulyqLgkj)


## Create a SignalR hub

> A hub is a class that serves as a high-level pipeline that handles client-server communication. 

```c# 
using System;
using System.Threading.Tasks;
using ChartPro.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;


namespace SignalRSimpleChat
{
    public class Chat : Hub
    {
        private readonly ChatDbContext _context;
        public Chat(ChatDbContext context)
        {
            _context = context;
        }
        public async Task Send( string message ,string UserName)
        {
            MessageHistory messageHistory = new MessageHistory
            {
                Message = message,
                UserName = "You",
                MessageDate = DateTime.Now,
                UserId = UserName
            };

            _context.MessageHistories.Add(messageHistory);
            _context.SaveChanges();
            await Clients.All.SendAsync("Send",  message);
           
        }
    }
}

```

The `Chat` class inherits from the SignalR Hub class. The Hub class manages connections, groups, and messaging.

The SendMessage method can be called by a connected client to send a message to all clients. JavaScript client code that calls the method is shown later in the tutorial. SignalR code is asynchronous to provide maximum scalability
## Configure SignalR
> The SignalR server must be configured to pass SignalR requests to SignalR.

- Add the following highlighted code to the Startup.cs file.

```c# 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChartPro.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SignalRSimpleChat;

using Microsoft.EntityFrameworkCore;

namespace ChartPro
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ChatDbContext>(options =>
            options.UseSqlServer(this.Configuration.GetConnectionString("DefaultConnection")));

            services.AddSession();
            services.AddControllersWithViews();
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                //endpoints.MapHub<MessagesHub>("/MessagesHub");
            });
           
            app.UseSignalR(routes =>
            {
                routes.MapHub<Chat>("/chat");
            });
        }
    }
}

```

- These changes add SignalR to the ASP.NET Core dependency injection and routing systems.


## Add SignalR client code

> the content in \view\home\Index.cshtml with the following code:


```javascript
<script src="https://chat-girls.pro/app-assets/chat/vendor/bundle.js"></script>
    <script src="https://chat-girls.pro/app-assets/chat/dist/js/app.min.js"></script>
    <script src="lib/signalr/signalr.min.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            let userName = $("#UserName").val();
            $.ajax({
                type: 'Get',
                url: '/Home/GetHistoryMessages',
                dataType: 'json',
                success: function (result) {
                    $.each(result, function (key, value) {
                        if (value.userName == "You") {
                            $('#messages').append('<div class="message-item  outgoing-message  "><div class="message-avatar"> <figure class="avatar"><a href="https://chat-girls.pro/app-assets/chat/images/nophoto.png" data-toggle="lightbox"><img src="https://chat-girls.pro/app-assets/chat/images/nophoto.png" class="rounded-circle" alt="You@elseLibby Kathi"></a></figure><div><h5>You</h5><div class="time">' + formatJSONDate(value.messageDate) + '</div></div></div><div class="message-content">' + value.message + '</div></div>');
                            scrollfunction();
                        } else if (value.userName == userName) {
                            $('#messages').append('<div id="lead_form" data-sid="outgoing267b-2326e4521-4521f-267bc-2326e4521f"><div id="body-outgoing267b-2326e4521-4521f-267bc-2326e4521f" style="display:none;">undefined</div><div class="message-item  message-outgoing267b-2326e4521-4521f-267bc-2326e4521f"><div class="message-avatar"><figure class="avatar"><img src="https://steamcdn-a.akamaihd.net/steamcommunity/public/images/avatars/b5/b5ea76ada823f6110bd6b75fc8782a92874e6825_full.jpg" class="rounded-circle"></figure><div><h5>Libby Kathi</h5><div class="time" style="display:block" id="timeoutgoing267b-2326e4521-4521f-267bc-2326e4521f"> ' + formatJSONDate(value.messageDate) + ' </div></div></div><div class="message-content" style="display:block" id="outgoing267b-2326e4521-4521f-267bc-2326e4521f"><span class="text-typing">' + value.message + '</span></div></div></div>');
                            scrollfunction();
                        }
                    });

                }
            });
           
        })
    </script>
    <script>
        const connection = new signalR.HubConnectionBuilder()
            .withUrl("/chat")
            .build();

        connection.start().catch(err => console.error(err.toString()));

        connection.on('Send', (message, UserName) => {
            appendLine(message, UserName);
        });

        document.getElementById('message_form').addEventListener('submit', event => {
            let message = $('#message').val();
            let = UserName = $('#UserName').val();
            $('#message').val('');

            connection.invoke('Send', message, UserName);
            event.preventDefault();
        });

        function appendLine(message, color) {
            let msgElement = document.createElement('em');
            msgElement.innerText = ` ${message}`;

            let li = document.createElement('li');
            li.appendChild(msgElement);
            var today = new Date();
            var time = today.getHours() + ":" + today.getMinutes();
            var times = today.getTime;
            var divid = today.getSeconds() + Math.floor((Math.random() * 100000) + 1);
            //$('#messages').append(span);
            $('#messages').append('<div class="message-item  outgoing-message  "><div class="message-avatar"> <figure class="avatar"><a href="https://chat-girls.pro/app-assets/chat/images/nophoto.png" data-toggle="lightbox"><img src="https://chat-girls.pro/app-assets/chat/images/nophoto.png" class="rounded-circle" alt="You@elseLibby Kathi"></a></figure><div><h5>You</h5><div class="time">' + formatAMPM(new Date) + '</div></div></div><div class="message-content">' + li.textContent + '</div></div>');
            setTimeout(function myfunction() {
                $("#messages").append('<div class="message-item  message-outgoingd365-263efc731-c7317-d365f-263efc7317" id="' + divid + '"><div class="message-avatar"><figure class="avatar"><img src="https://steamcdn-a.akamaihd.net/steamcommunity/public/images/avatars/b5/b5ea76ada823f6110bd6b75fc8782a92874e6825_full.jpg" class="rounded-circle"></figure><div><h5>Libby Kathi</h5><div class="time" style="display:none" id="timeoutgoingd365-263efc731-c7317-d365f-263efc7317">' + formatAMPM(new Date) + ' </div></div></div><div class="message-content" style="display:block" id="outgoingd365-263efc731-c7317-d365f-263efc7317"><span class="text-typing">Typing...</span></div></div>')
                scrollfunction();
            }, 800);
            scrollfunction();
            DefaultMessage(divid);

        };
        function DefaultMessage(divid) {
            $.ajax({
                type: 'Get',
                url: '/Home/DefaultMsg',
                dataType: 'json',
                success: function (result) {
                    setTimeout(function myfunction() {
                        if (result == "Typing....") {
                            $("#" + divid + "").show();
                        } else {
                            $("#" + divid + "").hide();
                            $('#messages').append('<div id="lead_form" data-sid="outgoing267b-2326e4521-4521f-267bc-2326e4521f"><div id="body-outgoing267b-2326e4521-4521f-267bc-2326e4521f" style="display:none;">undefined</div><div class="message-item  message-outgoing267b-2326e4521-4521f-267bc-2326e4521f"><div class="message-avatar"><figure class="avatar"><img src="https://steamcdn-a.akamaihd.net/steamcommunity/public/images/avatars/b5/b5ea76ada823f6110bd6b75fc8782a92874e6825_full.jpg" class="rounded-circle"></figure><div><h5>Libby Kathi</h5><div class="time" style="display:block" id="timeoutgoing267b-2326e4521-4521f-267bc-2326e4521f"> ' + formatAMPM(new Date) + ' </div></div></div><div class="message-content" style="display:block" id="outgoing267b-2326e4521-4521f-267bc-2326e4521f"><span class="text-typing">' + result + '</span></div></div></div>');
                        }
                        if (result == null) {
                            $("#" + divid + "").show();
                        }
                        scrollfunction();
                    }, 5000);
                }
            });
        }
        function formatAMPM(date) {
            var hours = date.getHours();
            var minutes = date.getMinutes();
            var ampm = hours >= 12 ? 'PM' : 'AM';
            hours = hours % 12;
            hours = hours ? hours : 12; // the hour '0' should be '12'
            minutes = minutes < 10 ? '0' + minutes : minutes;
            var strTime = hours + ':' + minutes + ' ' + ampm;
            return strTime;
        }
        function scrollfunction() {
            var objDiv = document.getElementById("chatbox");
            objDiv.scrollTop = objDiv.scrollHeight;
        }
        function formatJSONDate(jsonDate) {
            var date = new Date(jsonDate);
            var hours = date.getHours();
            var minutes = date.getMinutes();
            var ampm = hours >= 12 ? 'PM' : 'AM';
            hours = hours % 12;
            hours = hours ? hours : 12; // the hour '0' should be '12'
            minutes = minutes < 10 ? '0' + minutes : minutes;
            var strTime = hours + ':' + minutes + ' ' + ampm;
            return strTime;
        }
    </script>
```

## Installation

- All the `code` required to get started
- Images of what it should look like

### Clone

- Clone this repo to your local machine using `https://github.com/dipakibl/signalRBoatChatModule.git`

### Setup

> now restore, build and run project.

```shell
$ dotnet restore
$ dotnet build
$ dotnet build
dotnet run
``` 


