#!/bin/sh

set -e

sed "s|\${REDIS_PASS}|${REDIS_PASS}|g" /etc/redis/redis.conf.template \
  > /etc/redis/redis.conf

exec redis-server /etc/redis/redis.conf