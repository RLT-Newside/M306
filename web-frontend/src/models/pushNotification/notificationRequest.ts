export class NotificationRequest {
  userId: string;
  title: string;
  body: string;
  data: Record<string, any>;

  constructor(userId: string, title: string, body: string, data: Record<string, any>) {
    this.userId = userId;
    this.title = title;
    this.body = body;
    this.data = data;
  }
}
