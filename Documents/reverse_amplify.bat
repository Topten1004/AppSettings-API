
ffmpeg -y -i %1 audio.wav
REM ffmpeg -y -i audio.3gp audio.wav
REM ffmpeg -normalize audio.wav -o audio_normalized.wav
REM ffmpeg -y -i "%1" -vn -c:a pcm_f32le -f wav -af "dynaudnorm=p=1/sqrt(2):m=100:s=12:g=15" %~n1_dn.wav"
REM ffmpeg -i input.mkv -af "dynaudnorm=p=0.71:m=100:s=12:g=15" -vcodec copy output.mkv
REM ffmpeg -y -i audio.wav -af "dynaudnorm=p=0.71:m=100:s=12:g=15" audio_normalized.wav
REM GUT ffmpeg -y -i audio.wav -af "dynaudnorm=f=150:g=15" audio_normalized.wav
REM GUT ffmpeg -y -i audio.wav -af "dynaudnorm=f=150:g=3" audio_normalized.wav
ffmpeg -y -i audio.wav -af "dynaudnorm=f=10:g=3" audio_normalized1.wav
ffmpeg -y -i audio_normalized1.wav -af "dynaudnorm=f=54:g=5" audio_normalized2.wav


sox -V audio_normalized2.wav backwards.wav reverse
ffmpeg -y -i backwards.wav %2
ffmpeg -y -i %2 %2.wav