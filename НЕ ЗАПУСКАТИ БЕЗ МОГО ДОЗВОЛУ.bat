docker login -u ihornet2024 -p dckr_263Wpat0xqg_HZrRH7nGUd6XkezuwPd_sWNPp6o

docker tag booking-logger_api ihornet2024/booking_logger_api:latest
docker tag booking-client_api ihornet2024/booking_client_api:latest
docker tag booking-email_api ihornet2024/booking_email_api:latest
docker tag booking-token_api ihornet2024/booking_token_api:latest
docker tag booking-residence_api ihornet2024/booking_residence_api:latest


docker push ihornet2024/booking_logger_api:latest
docker push ihornet2024/booking_client_api:latest
docker push ihornet2024/booking_email_api:latest
docker push ihornet2024/booking_token_api:latest
docker push ihornet2024/booking_residence_api:latest
