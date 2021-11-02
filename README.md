# Networking between Unity and TouchDesigner
Example of networking between unity-server-td using SocketIO and OSC protocol.  
Unity is for remote controller, TouchDesigner is for player.  
SocketIO and Nodejs are not necessery in this case, but used it for studying.

## Install 
Download Node.js, run this with following commands:  
```python
# Install dependencies (only first time)
npm install

# Run the server
node server
```

## Basics
TD  
- OSC In : listener. waiting for the server send message.  
- OSC Out : emitter. send message to server.

## Server
Detail explanation of server.js code.
```python
# Intalled dependencies. (cron is for scheduling, didn't need it here.)  
const express = require('express')
const app = express()
const http = require('http').Server(app)
const io = require('socket.io')(http)
const osc = require('node-osc')
const cron = require('node-cron')
  
# Socket port is set by window default. As we don't have setting panel here, keep it 8000. Can check the port with unity at your computer when it's running.  
const SOCKETIO_PORT = 8888
const OSC_CLIENT_PORT = 8000
const OSC_SERVER_PORT = 8001

# Make nodejs server > SocketIO is listening  
http.listen(SOCKETIO_PORT, '0.0.0.0', function(){
  console.log('SocketIO listening on :', SOCKETIO_PORT)
})

# Server is using port for listening OSC from TD.  
const OSC_SERVER = new osc.Server(OSC_SERVER_PORT, '127.0.0.1', () => {
  console.log('OSC Server is listening on :', OSC_SERVER_PORT);
})

# Declaration osc client for sending message to TD.  
const OSC_CLIENT = new osc.Client('0.0.0.0', OSC_CLIENT_PORT)

# SocketIO listener. when unity is connected, > 'client is connected' 
io.on('connection', function(socket){
  console.log(`client is connected`)
  ...
})

# SocketIO listening data from unity, when it arrives send it to TD.
socket.on('player:send-player-event', function(data){
    console.log(data.cmd)
    OSC_CLIENT.send('/player/osc_listener', data.cmd, function () {})
  })
  
#  SocketIO listening message from TD. when it arrives send it to unity.
OSC_SERVER.on('message', function (msg) {
    console.log('new message from TD:')
    console.log(msg)
    socket.emit('player:get-player-event', { 'cmd' : msg[0], 'value' : msg[1] })
  })
``` 
server console example  
![image](https://user-images.githubusercontent.com/64575677/139779206-146a2ceb-a4cc-4b48-a4b8-a551af3e64ef.png)





