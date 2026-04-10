import { PersonSearchResult as CreatePersonRequest } from '@/models/profilePictureReview/profilePicture';
import { profilePictureAlova } from '../alova';

export const searchPerson = (searchProperty: string, searchValue: string) => {
  return profilePictureAlova.Get<CreatePersonRequest[]>('manualRequestGeneration', {
    params: { searchProperty, searchValue },
  });
};

export const createRequest = (person: CreatePersonRequest) => {
  return profilePictureAlova.Post<CreatePersonRequest>(
    'manualRequestGeneration',
    JSON.stringify(person)
  );
};
