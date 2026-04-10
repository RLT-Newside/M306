import { studentsManualAlova } from '../alova';
import type { Article } from '@/models/studentsManual/article';
import type { Attachment } from '@/models/studentsManual/articleResource';

// Get all articles
export const getArticles = () => studentsManualAlova.Get<Article[]>('article');

// Get article with articleId
export const getArticle = (articleId: string) =>
  studentsManualAlova.Get<Article>(`article/${articleId}`);

// Create new article
export const createArticle = (article: Article) =>
  studentsManualAlova.Post<Article>('article', JSON.stringify(article));

// Update an existing article with given articleId
export const updateArticle = (articleId: string, article: Article) =>
  studentsManualAlova.Put<Article>(`article/${articleId}`, JSON.stringify(article));

// Delete article
export const deleteArticle = (articleId: string) =>
  studentsManualAlova.Delete(`article/${articleId}`);

// Add attachment to existing article
export const addAttachment = (articleId: string, attachment: Attachment) =>
  studentsManualAlova.Post<Attachment>(
    `article/${articleId}/attachments`,
    JSON.stringify(attachment)
  );

// Remove attachment from existing article
export const removeAttachment = (articleId: string, attachmentId: string) =>
  studentsManualAlova.Delete(`article/${articleId}/attachments/${attachmentId}`);
