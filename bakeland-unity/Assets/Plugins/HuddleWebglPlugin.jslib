    var NativeLib = {
    
    huddleClient: null,
    room : null,
    huddleToken:null,
    localStream : null,
    audioContext : null,
    audioListener : null,
    soundObjects : null,
    peersMap : null,
    autoConsume: true,
    

    // Video Receive
    NewTexture: function () {
        var tex = GLctx.createTexture();
        if (!tex){
            console.error("Failed to create a new texture for VideoReceiving")
            return LKBridge.NullPtr;
        }

        var id = GL.getNewId(GL.textures);
        tex.name = id;
        GL.textures[id] = tex;
        return id;
    },

    InitHuddle01WebSdk:function(projectId,shouldAutoConnectConsumer)
    {
        autoConsume = shouldAutoConnectConsumer;
        huddleClient = new HuddleWebCore.HuddleClient({
                        projectId: UTF8ToString(projectId),
                        options: {
                            activeSpeakers: {
                            // Number of active speaker visible in the grid, by default 8
                            size: 10,
                            },
                        },
                    });

        huddleClient.localPeer.on('receive-data', function (data) {
            console.log(data);
            SendMessage("HuddleClient", "MessageReceived",JSON.stringify(data));
        });

        peersMap = new Map();

        window.addEventListener("beforeunload", function (event) {
           
            if (typeof unityInstance !== 'undefined' && unityInstance) {
                huddleClient.leaveRoom();
                SendMessage("HuddleClient", "OnLeavingRoom");
            }
        });
    },

    JoinRoom : async function(roomId,tokenVal)
    {
        console.log("Join Room");
        //join room
        room = await huddleClient.joinRoom({
                roomId: UTF8ToString(roomId),
                token: UTF8ToString(tokenVal),
                });


        // on join room event
        room.on("room-joined", async function () {

            console.log("Room ID:", room.roomId);
            const remotePeers = huddleClient.room.remotePeers
            SendMessage("HuddleClient", "OnRoomJoined"); 
            
            //if peer already exists
            for (const entry of Array.from(remotePeers.entries())) {
                (function(entry) {
                    const key = entry[0];
                    const value = entry[1];
                    const tempRemotePeer = value;
                    const peerIdString = tempRemotePeer.peerId;

                    peersMap[peerIdString] = { audioStream: null, videoStream: null };
                    SendMessage("HuddleClient", "OnPeerAdded", peerIdString);

                    // remote peer on metadata updated event
                    tempRemotePeer.on("metadata-updated", async function () {
                        try {
                            const updatedMetadata = await huddleClient.room.getRemotePeerById(peerIdString).getMetadata();
                            SendMessage("HuddleClient", "OnPeerMetadataUpdated", peerIdString);
                        } catch (error) {
                            console.error("Error updating metadata: ", error);
                        }
                    });

                if(autoConsume)
                {
                            // remote peer on stream available event
                    tempRemotePeer.on("stream-playable", async function(data) {
                        if (data.label == "audio") {
                            const audioElem = document.createElement("audio");
                            audioElem.id = peerIdString + "_audio";

                            if (!data.consumer.track) {
                                return console.log("track not found");
                            }

                            const stream = new MediaStream([data.consumer.track]);
                            document.body.appendChild(audioElem);

                            audioElem.srcObject = stream;
                            audioElem.play();

                            if (peersMap[peerIdString]) {
                                peersMap[peerIdString].audioStream = stream;
                            }

                            SendMessage("HuddleClient", "OnPeerUnMute", peerIdString);

                        } else if (data.label == "video") {
                            if (!data.consumer.track) {
                                return console.log("track not found");
                            }

                            const videoElem = document.createElement("video");
                            videoElem.id = peerIdString + "_video";

                            document.body.appendChild(videoElem);

                            videoElem.style.display = "none";
                            videoElem.style.opacity = 0;
                            const videoStream = new MediaStream([data.consumer.track]);
                            videoElem.srcObject = videoStream;
                            videoElem.play();
                            SendMessage("HuddleClient", "ResumeVideo", peerIdString);
                        }
                    });

                        // remote peer on stream closed event
                        tempRemotePeer.on("stream-closed", function(data) {
                            if (data.label == "audio") {
                                const audioElem = document.getElementById(peerIdString + "_audio");
                                console.log("audio on audio close", audioElem);
                                if (audioElem) {
                                    audioElem.srcObject = null;
                                    audioElem.remove();
                                }

                                if (peersMap[peerIdString]) {
                                    peersMap[peerIdString].audioStream = null;
                                }

                            SendMessage("HuddleClient", "OnPeerMute", peerIdString);

                            } else if (data.label == "video") {
                                const videoElem = document.getElementById(peerIdString + "_video");

                                if (videoElem) {
                                    videoElem.remove();
                                }

                                SendMessage("HuddleClient", "StopVideo", peerIdString);
                            }
                    });

                }
            
                // metadata already exist
                (async function() {
                    try {
                        const updatedMetadata = await huddleClient.room.getRemotePeerById(peerIdString).getMetadata();
                        SendMessage("HuddleClient", "OnPeerMetadataUpdated", peerIdString);
                    } catch (error) {
                        console.error("Error fetching initial metadata: ", error);
                    }
                })();
            })(entry); // Immediately Invoked Function Expression (IIFE) to capture the correct peerId
            }
        });

        
        //room-closed event
        room.on("room-closed", function () {
            console.log("Peer ID:", data.peerId);
            SendMessage("HuddleClient", "OnRoomClosed");     
        });

        //new-peer-joined event
        huddleClient.room.on("new-peer-joined", function (data) {
        
            console.log("new-peer-joined Peer ID:", data.peer);
            
            peersMap[data.peer.peerId] = { audioStream: null, videoStream:null };
            SendMessage("HuddleClient", "OnPeerAdded",data.peer.peerId);
       
        
            var remotePeer = data.peer;

            remotePeer.on("metadata-updated", async function () {
                console.log("Successfully updated remote peer metadata of : ", remotePeer.peerId);
                var updatedMetadata = await huddleClient.room.getRemotePeerById(remotePeer.peerId).getMetadata();
                SendMessage("HuddleClient", "OnPeerMetadataUpdated",remotePeer.peerId);

            });
        
            remotePeer.on("stream-playable", async function(data) 
            {

                if(data.label == "audio")
                {
                    const audioElem = document.createElement("audio");
                    audioElem.id = remotePeer.peerId + "_audio";

                    if(!data.consumer.track)
                    {
                        return console.log("track not found");    
                    }
                    
                    const stream  = new MediaStream([data.consumer.track]);
                    
                    document.body.appendChild(audioElem);
                    audioElem.srcObject = stream;
                    audioElem.play();
                    
                    if(peersMap[remotePeer.peerId])
                    {
                        peersMap[remotePeer.peerId].audioStream = stream;
                    }

                    SendMessage("HuddleClient", "OnPeerUnMute",remotePeer.peerId);

                }else if(data.label == "video")
                {
                    var videoElem = document.createElement("video");
                    videoElem.id = remotePeer.peerId + "_video";
                    console.log("video created : ",videoElem.id);
                    document.body.appendChild(videoElem);
                    //set properties
                    videoElem.style.display = "none";
                    videoElem.style.opacity = 0;
                    //get stream
                    const videoStream  = new MediaStream([data.consumer.track]);
                    videoElem.srcObject = videoStream;
                    videoElem.play();
                    SendMessage("HuddleClient", "ResumeVideo",remotePeer.peerId);
                }
            });
        
            remotePeer.on("stream-closed", function (data) {
                console.log("Remote Peer Stream is closed.",data);

                if(data.label == "audio")
                {
                    var audioElem = document.getElementById(remotePeer.peerId+"_audio");
                    if(audioElem)
                    {
                        audioElem.srcObject = null;
                        audioElem.remove();
                    }

                    if(peersMap[remotePeer.peerId])
                    {
                        peersMap[remotePeer.peerId].audioStream = null;
                    }

                    SendMessage("HuddleClient", "OnPeerMute",remotePeer.peerId);

                }else if(data.label == "video")
                {
                    var videoElem = document.getElementById(remotePeer.peerId + "_video");

                    if(videoElem)
                    {
                        videoElem.remove();
                    }

                    SendMessage("HuddleClient", "StopVideo",remotePeer.peerId);

                }
            });
        
        });

        //peer-left
        room.on("peer-left", function (peerId) {
            console.log(" peer-left Peer ID:", peerId);

            SendMessage("HuddleClient", "OnPeerLeft",peerId);

            //remove audio element
            var audioElem = document.getElementById(peerId+"_audio");

            if(audioElem)
            {
                audioElem.srcObject = null;
                audioElem.remove();
            }

            //remove video element
            var videoElem = document.getElementById(peerId + "_video");

            if(videoElem)
            {
                videoElem.remove();
            }

            // delete associated
                 
            peersMap.delete(UTF8ToString(peerId));
        });
    },


    EnableAudio : async function()
    {
        await huddleClient.localPeer.enableAudio();
    },

    DisableAudio : async function()
    {
        await huddleClient.localPeer.disableAudio();
    },

    EnableVideo : async function()
    {
        localStream = await huddleClient.localPeer.enableVideo();

        var videoElem = document.createElement("video");
        videoElem.id = huddleClient.localPeer.peerId + "_video";
        console.log("video created : ",videoElem.id);
        document.body.appendChild(videoElem);

        videoElem.srcObject = localStream;
        videoElem.play();

        SendMessage("HuddleClient", "ResumeVideo",huddleClient.localPeer.peerId);
    },

    DisableVideo : async function()
    {
        await huddleClient.localPeer.disableVideo();

        var videoElem = document.getElementById(huddleClient.localPeer.peerId + "_video");

        if(videoElem)
        {
            videoElem.remove();
        }

        SendMessage("HuddleClient", "StopVideo",huddleClient.localPeer.peerId);
    },
    
    LeaveRoom : function()
    {
        for (var entry of Array.from(peersMap.entries()))
       {
            var key = entry[0];
            var value = entry[1];
            var audioElem = document.getElementById(key+"_audio");
            //remove element
            if(audioElem)
            {
                audioElem.srcObject = null;
                audioElem.remove();
            }
            var videoElem = document.getElementById(UTF8ToString(peerId) + "_video");

            if(videoElem)
            {
                videoElem.remove();
            }
       }

       peersMap = new Map();

       huddleClient.leaveRoom();
       SendMessage("HuddleClient", "OnLeavingRoom");
       //remove all audio associated
    },

    SendTextMessage : function(message)
    {
        var mes = UTF8ToString(message);
        console.log("Sending message",mes);
        huddleClient.localPeer.sendData({ to: "*", payload: mes, label: 'chat' });
    },

    ConsumePeer : function(peerId,label)
    {
        huddleClient.localPeer.consume(peerId,label);
    },

    StopConsumingPeer : function(peerId,label)
    {
        huddleClient.localPeer.stopConsuming(peerId,label);
    },

    GetAudioMediaDevices : async function()
    {
         var mediaDevices = await huddleClient.localPeer.deviceHandler.getMediaDevices('mic')
        .then(md => {
            console.log(md);
            SendMessage("HuddleClient", "OnAudioDevicesReceived",JSON.stringify(md));

        })
        .catch(error => {
            console.error("Error fetching media devices:", error);
        });
        
    },

    GetVideoMediaDevices : async function()
    {
        var mediaDevices = await huddleClient.localPeer.deviceHandler.getMediaDevices('cam')
        .then(md => {
            console.log(md);
            SendMessage("HuddleClient", "OnVideoDevicesReceived",JSON.stringify(md));
        })
        .catch(error => {
            console.error("Error fetching media devices:", error);
        });
        
    },

    UpdatePeerMetaData : function(metadataVal)
    {
        var metadata = JSON.parse(UTF8ToString(metadataVal));
        huddleClient.localPeer.updateMetadata({ 
            name : metadata.name
        });
    },

    GetRemotePeerMetaData : function(peerId)
    {
        console.log("GetRemotePeerMetaData : " );
        var remotePeer = huddleClient.room.getRemotePeerById(UTF8ToString(peerId)).getMetadata();
        console.log("remote Peer Metadata : " ,remotePeer );

        var bufferSize = lengthBytesUTF8(JSON.stringify(remotePeer)) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(JSON.stringify(remotePeer), buffer, bufferSize);
        return buffer;
    },

    GetLocalPeerId : function()
    {
        var peerId = huddleClient.localPeer.peerId;
        console.log("Local peer Id : ",peerId);
        var bufferSize = lengthBytesUTF8(peerId) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(peerId, buffer, bufferSize);
        return buffer;
        
    },

    AttachVideo: function (peerId, texId) {
        var tex = GL.textures[texId];
        var lastTime = -1;
        var peerIdString = UTF8ToString(peerId);
        console.log("video id " + UTF8ToString(peerId) + "_video");
        var initialVideo = document.getElementById(UTF8ToString(peerId) + "_video");
        initialVideo.style.opacity = 0;
        initialVideo.style.width = 0;
        initialVideo.style.height = 0;
        initialVideo.style.display = "none";
        setTimeout(function() {
            initialVideo.play();
        }, 0)
        initialVideo.addEventListener("canplay", (event) => {
            initialVideo.play();
        });
 
        //document.body.appendChild(initialVideo);
        var updateVideo = function (peerIdVal,textureId) {
            
            var video = document.getElementById(peerIdVal + "_video");

            if (!video || video === undefined) {
                initialVideo.remove();
                return;
            }
            
            if (!video.paused) {
                
                GLctx.bindTexture(GLctx.TEXTURE_2D, tex);
                
                // Flip Y
                GLctx.pixelStorei(GLctx.UNPACK_FLIP_Y_WEBGL, true);
                GLctx.texImage2D(GLctx.TEXTURE_2D, 0, GLctx.RGBA, GLctx.RGBA, GLctx.UNSIGNED_BYTE, video);
                GLctx.pixelStorei(GLctx.UNPACK_FLIP_Y_WEBGL, false);

                GLctx.texParameteri(GLctx.TEXTURE_2D, GLctx.TEXTURE_MAG_FILTER, GLctx.LINEAR);
                GLctx.texParameteri(GLctx.TEXTURE_2D, GLctx.TEXTURE_MIN_FILTER, GLctx.LINEAR);
                GLctx.texParameteri(GLctx.TEXTURE_2D, GLctx.TEXTURE_WRAP_S, GLctx.CLAMP_TO_EDGE);
                GLctx.texParameteri(GLctx.TEXTURE_2D, GLctx.TEXTURE_WRAP_T, GLctx.CLAMP_TO_EDGE);
            }
            
            requestAnimationFrame(function(){updateVideo(peerIdVal,textureId)});
        };
        
        requestAnimationFrame(function(){updateVideo(peerIdString,tex)});
    },
};

mergeInto(LibraryManager.library, NativeLib);

