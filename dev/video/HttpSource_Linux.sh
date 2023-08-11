#cvlc -v v4l2:///dev/video0 \
#  --sout \
#   "#transcode{vcodec=h264, vb=800, acodec=none}
#    :rtp{sdp=rtp://@:5600/}"

#cvlc v4l2:///dev/video0:chroma=YUY2:width=640:height=480 --sout '#transcode{vcodec=h264}:rtp{dst=127.0.0.1,port=5600,sdp=\}' --no-audio

#cvlc -vvv v4l2:///dev/video0:width=640:height=480 --sout '#transcode{vcodec=h264}:rtp{dst=127.0.0.1,port=5600,ttl=1}' --no-audio

cvlc -vvv v4l2:///dev/video0:width=640:height=480 \
  --sout "#transcode{vcodec=h264,acodec=none,scodec=none}:http{dst=192.168.170.12:8080/test.wmv}" --no-audio --no-sout-all #--sout-keep

#cvlc -vvv v4l2:///dev/video0:width=640:height=480 \
#  --sout "#transcode{vcodec=h264,acodec=none,scodec=none}:rtp{dst=192.168.114.217,port=8080,mux=ts}" --no-sout-all --sout-keep

#cvlc -vvv "v4l2:///dev/video2:width=1920:height=1920" \
#  --sout "#transcode{vcodec=h264,acodec=none,scodec=none}:http{dst=192.168.9.12:8080/test.wmv}" --no-sout-all --sout-keep

#cvlc -vvv "v4l2:///dev/video2:width=1920:height=1920" \
#  --sout "#transcode{vcodec=h264,acodec=none,scodec=none}:rtp{dst=192.168.114.217,port=8080,mux=ts}" --no-sout-all --sout-keep
