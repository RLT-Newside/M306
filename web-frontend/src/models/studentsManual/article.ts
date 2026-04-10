import type { Attachment } from './articleResource';

export class Article {
  id: string;
  title: string;
  content: string;
  viewCounter: number;
  attachments: Attachment[];
  tags: string[];

  constructor(
    id: string,
    title: string,
    content: string,
    viewCounter: number,
    attachments: Attachment[],
    tags: string[]
  ) {
    this.id = id;
    this.title = title;
    this.content = content;
    this.viewCounter = viewCounter;
    this.attachments = attachments;
    this.tags = tags;
  }
}
