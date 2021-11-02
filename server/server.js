const express = require('express')
const app = express()
const http = require('http').Server(app)
const io = require('socket.io')(http)
const osc = require('node-osc')
const cron = require('node-cron')

const SOCKETIO_PORT = 8888
const OSC_CLIENT_PORT = 8000
const OSC_SERVER_PORT = 8001

http.listen(SOCKETIO_PORT, '0.0.0.0', function(){
  console.log('SocketIO listening on :', SOCKETIO_PORT)
})

const OSC_SERVER = new osc.Server(OSC_SERVER_PORT, '127.0.0.1', () => {
  console.log('OSC Server is listening on :', OSC_SERVER_PORT);
})
const OSC_CLIENT = new osc.Client('0.0.0.0', OSC_CLIENT_PORT)

io.on('connection', function(socket){

  console.log(`client is connected`)

  socket.on('player:send-player-event', function(data){
    console.log(data.cmd)
    OSC_CLIENT.send('/player/osc_listener', data.cmd, function () {})
  })

  OSC_SERVER.on('message', function (msg) {
    console.log('new message from TD:')
    console.log(msg)
    socket.emit('player:get-player-event', { 'cmd' : msg[0], 'value' : msg[1] })
  })

})
