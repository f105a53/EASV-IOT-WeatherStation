sudo rm stream* -vfr; sudo ffmpeg -i /dev/video0 -input_format h264 -f hls -g 2 -hls_playlist_type event -r 2 stream.m3u8
