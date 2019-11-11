sudo raspivid -o - -t 0 -fps 30 -g 50 -n -a 12 -b 25000000 | sudo ffmpeg -re -f h264 -i - -c:v copy -f flv rtmp://localhost/show/stream
