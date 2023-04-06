all:
	docker build -t kip-profile .
	docker pull mongo
	docker volume create kip_profile_mongodb_data
	docker compose up