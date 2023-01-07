all:
	docker build -t kip-profile .
	docker pull mongo
	docker compose up