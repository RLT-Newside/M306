# Creates the image to host the Frontend with
# Uses nginx as a webserver to do so

FROM node:lts-alpine AS build-stage
WORKDIR /app
COPY package*.json ./

# Install dependencies and build app.
RUN npm install
COPY . .
RUN npm run build


# production stage
FROM nginx:stable-alpine as production-stage
RUN mkdir /app
COPY --from=build-stage /app/dist /app
COPY nginx.conf /etc/nginx/nginx.conf
EXPOSE 80

# Override the default NGINX container behaviour to substitute the statically compiled environment variables
COPY ./substitute_environment_variables.sh /substitute_environment_variables.sh
RUN chmod +x /substitute_environment_variables.sh
ENTRYPOINT [ "/substitute_environment_variables.sh" ]
