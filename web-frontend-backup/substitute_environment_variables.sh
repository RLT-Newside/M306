#!/bin/sh
ROOT_DIR=/app

# Replace env vars in files served by NGINX
for file in $ROOT_DIR/assets/*.js* $ROOT_DIR/index.html;
do
  echo "Processing: $file";

  sed -i 's|PLACEHOLDER_VALUE_GIBZAPP_API_HOST|'${GIBZAPP_API_HOST}'|g' $file
  sed -i 's|PLACEHOLDER_VALUE_GIBZAPP_API_BASE_PATH|'${GIBZAPP_API_BASE_PATH}'|g' $file
  sed -i 's|PLACEHOLDER_VALUE_GIBZAPP_API_TIMEOUT_MS|'${GIBZAPP_API_TIMEOUT_MS}'|g' $file
  sed -i 's|PLACEHOLDER_VALUE_GIBZAPP_API_PROFILE_PICTURE_ENDPOINT|'${GIBZAPP_API_PROFILE_PICTURE_ENDPOINT}'|g' $file
  # Add other variables here...
done

# Start NGINX
echo "Starting NGINX...";
nginx -g 'daemon off;'