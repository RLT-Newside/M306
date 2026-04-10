import type { AxiosResponse, AxiosError } from 'axios';
import type { Article } from '../../models/studentsManual/article';
import BaseApi from '../baseApi';

class ArticleApi extends BaseApi {
  public getAll(): Promise<Article[]> {
    return this.get<Article[]>('students-manual/v1/article')
      .then<Article[]>(ArticleApi.then)
      .catch<Article[]>(ArticleApi.catch);
  }

  public create(article: Partial<Article>): Promise<Article> {
    return this.post<Article>('students-manual/v1/article', JSON.stringify(article))
      .then<Article>(ArticleApi.then)
      .catch<Article>(ArticleApi.catch);
  }

  public update(articleId: string, topic: Article): Promise<Article> {
    return this.put<Article>(`students-manual/${articleId}`, JSON.stringify(topic))
      .then<Article>(ArticleApi.then)
      .catch<Article>(ArticleApi.catch);
  }

  public remove(articleId: string): Promise<void> {
    return this.delete<void>(`students-manual/${articleId}`)
      .then<void>(ArticleApi.then)
      .catch<void>(ArticleApi.catch);
  }

  private static then<T>(response: AxiosResponse<T>) {
    return response.data;
  }

  private static catch<T>(error: AxiosError): T {
    throw error;
  }
}

export const articleApi = new ArticleApi();
