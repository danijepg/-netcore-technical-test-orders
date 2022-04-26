CREATE TABLE "Orders" (
	"event_id" uuid NOT NULL,
	"order_id" uuid NOT NULL,
	"payload" jsonb NOT NULL,
	CONSTRAINT pk_order_events PRIMARY KEY ("event_id")
);