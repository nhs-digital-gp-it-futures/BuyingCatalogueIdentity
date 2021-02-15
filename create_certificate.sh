mkcertVersion="v1.4.3"

sudo apt update
sudo apt install libnss3-tools wget -y

mkdir mkcert
cd mkcert

if [ ! -f "mkcert" ]; then
    wget "https://github.com/FiloSottile/mkcert/releases/download/$mkcertVersion/mkcert-$mkcertVersion-linux-amd64"
    mv mkcert-v1.4.3-linux-amd64 mkcert
    chmod +x mkcert
fi

./mkcert -install
./mkcert -pkcs12 -p12-file localhost.pfx localhost host.docker.internal 127.0.0.1

cp "$(./mkcert -CAROOT)/rootCA.pem" rootCA.pem
