import type { RouteRecordRaw } from 'vue-router';
import { createRouter, createWebHistory } from 'vue-router';
import { useAuthenticationStore } from '@/stores/authentication';

// Only eagerly load the home page for fast initial render
import Home from '@/pages/Home.vue';

// Lazy load all other pages
const ShowManual = () => import('@/pages/studentsManual/ShowManual.vue');
const NewArticle = () => import('@/pages/studentsManual/NewArticle.vue');
const EditArticle = () => import('@/pages/studentsManual/EditArticle.vue');
const ListStages = () => import('@/pages/rally/ListStages.vue');
const NewStage = () => import('@/pages/rally/NewStage.vue');
const ShowStage = () => import('@/pages/rally/ShowStage.vue');
const ListRallies = () => import('@/pages/rally/ListRallies.vue');
const NewRally = () => import('@/pages/rally/NewRally.vue');
const ShowRally = () => import('@/pages/rally/ShowRally.vue');
const NewRallyStage = () => import('@/pages/rally/NewRallyStage.vue');
const ShowRallyStage = () => import('@/pages/rally/ShowRallyStage.vue');
const ShowConstraint = () => import('@/pages/rally/ShowConstraint.vue');
const NewStageActivity = () => import('@/pages/rally/NewStageActivity.vue');
const EditStageActivity = () => import('@/pages/rally/EditStageActivity.vue');
const EditStage = () => import('@/pages/rally/EditStage.vue');
const ListClients = () => import('@/pages/idsrv/ListClients.vue');
const NewClient = () => import('@/pages/idsrv/NewClient.vue');
const EditClient = () => import('@/pages/idsrv/EditClient.vue');
const ListApiResources = () => import('@/pages/idsrv/ListApiResources.vue');
const NewApiResource = () => import('@/pages/idsrv/NewApiResource.vue');
const ListApiScopes = () => import('@/pages/idsrv/ListApiScopes.vue');
const NewApiScope = () => import('@/pages/idsrv/NewApiScope.vue');
const ListRoles = () => import('@/pages/idsrv/ListRoles.vue');
const NewRole = () => import('@/pages/idsrv/NewRole.vue');
const ListUsers = () => import('@/pages/idsrv/ListUsers.vue');
const ShowUser = () => import('@/pages/idsrv/ShowUser.vue');
const DeeplinkLandingPage = () => import('@/pages/profilePicture/DeeplinkLandingPage.vue');
const ProfilePictureReview = () => import('@/pages/profilePicture/ProfilePictureReview.vue');
const ProfilePictureRequest = () => import('@/pages/profilePicture/ProfilePictureRequest.vue');
const ManualRequestGeneration = () => import('@/pages/profilePicture/ManualRequestGeneration.vue');
const DevelopmentSender = () => import('@/pages/pushNotification/DevelopmentSender.vue');
const DevTools = () => import('@/pages/DevTools.vue');
const SportsTestLeaderboard = () => import('@/pages/sportsTest/leaderboard.vue');

declare module 'vue-router' {
  interface RouteMeta {
    // is optional
    requiresAuthentication?: boolean;

    // must be declared by every route
    requiredRole?: string;

    // hide navigation and app bar for public pages
    isPublicPage?: boolean;
  }
}

const routes: Array<RouteRecordRaw> = [
  {
    path: '/',
    name: 'Home',
    component: Home,
  },
  {
    path: '/students-manual',
    name: 'Students manual',
    component: ShowManual,
  },
  {
    path: '/students-manual/new-article',
    name: 'New article',
    component: NewArticle,
  },
  {
    path: '/students-manual/:articleId',
    name: 'Edit article',
    component: EditArticle,
    props: true,
  },
  {
    path: '/rally/stages',
    name: 'List stages',
    component: ListStages,
    meta: { requiresAuthentication: true, requiredRole: 'gibzRallyManager' },
  },
  {
    path: '/rally/new-stage',
    name: 'New stage',
    component: NewStage,
    meta: { requiresAuthentication: true, requiredRole: 'gibzRallyManager' },
  },
  {
    path: '/rally/stage/:stageId',
    name: 'Show stage',
    component: ShowStage,
    props: true,
    meta: { requiresAuthentication: true, requiredRole: 'gibzRallyManager' },
  },
  {
    path: '/rally/stage/:stageId/edit',
    name: 'Edit stage',
    component: EditStage,
    props: true,
    meta: { requiresAuthentication: true, requiredRole: 'gibzRallyManager' },
  },
  {
    path: '/rally/stage/:stageId/new-activity',
    name: 'New stage activity',
    component: NewStageActivity,
    props: true,
    meta: { requiresAuthentication: true, requiredRole: 'gibzRallyManager' },
  },
  {
    path: '/rally/stage/:stageId/activity/:activityId',
    name: 'Edit stage activity',
    component: EditStageActivity,
    props: true,
    meta: { requiresAuthentication: true, requiredRole: 'gibzRallyManager' },
  },
  {
    path: '/rally/rallies',
    name: 'List rallies',
    component: ListRallies,
    meta: { requiresAuthentication: true, requiredRole: 'gibzRallyManager' },
  },
  {
    path: '/rally/new-rally',
    name: 'New rally',
    component: NewRally,
    meta: { requiresAuthentication: true, requiredRole: 'gibzRallyManager' },
  },
  {
    path: '/rally/rally/:rallyId',
    name: 'Show rally',
    component: ShowRally,
    props: true,
    meta: { requiresAuthentication: true, requiredRole: 'gibzRallyManager' },
  },
  {
    path: '/rally/rally/:rallyId/add-stage',
    name: 'Add rallyStage',
    component: NewRallyStage,
    props: true,
    meta: { requiresAuthentication: true, requiredRole: 'gibzRallyManager' },
  },
  {
    path: '/rally/rally/:rallyId/stage/:rallyStageId',
    name: 'Show rallyStage',
    component: ShowRallyStage,
    props: true,
    meta: { requiresAuthentication: true, requiredRole: 'gibzRallyManager' },
  },
  {
    path: '/rally/rally/:rallyId/stage/:rallyStageId/constraint/:constraintId',
    name: 'Show constraint',
    component: ShowConstraint,
    props: true,
    meta: { requiresAuthentication: true, requiredRole: 'gibzRallyManager' },
  },

  {
    path: '/id/roles',
    name: 'List Roles',
    component: ListRoles,
    props: true,
    meta: { requiresAuthentication: true, requiredRole: 'idsrvAdmin' },
  },
  {
    path: '/id/roles/new-role',
    name: 'New Role',
    component: NewRole,
    props: true,
    meta: { requiresAuthentication: true, requiredRole: 'idsrvAdmin' },
  },
  {
    path: '/id/users',
    name: 'List Users',
    component: ListUsers,
    props: true,
    meta: { requiresAuthentication: true, requiredRole: 'idsrvAdmin' },
  },
  {
    path: '/id/users/:userId',
    name: 'Show User',
    component: ShowUser,
    props: true,
    meta: { requiresAuthentication: true, requiredRole: 'idsrvAdmin' },
  },
  {
    path: '/idsrv/clients',
    name: 'List Clients',
    component: ListClients,
    props: true,
    meta: { requiresAuthentication: true, requiredRole: 'idsrvAdmin' },
  },
  {
    path: '/idsrv/clients/new-client',
    name: 'New Client',
    component: NewClient,
    props: true,
    meta: { requiresAuthentication: true, requiredRole: 'idsrvAdmin' },
  },
  {
    path: '/idsrv/clients/:clientId',
    name: 'Edit Client',
    component: EditClient,
    props: true,
    meta: { requiresAuthentication: true, requiredRole: 'idsrvAdmin' },
  },
  {
    path: '/idsrv/apiResources',
    name: 'List API Resources',
    component: ListApiResources,
    props: true,
    meta: { requiresAuthentication: true, requiredRole: 'idsrvAdmin' },
  },
  {
    path: '/idsrv/apiResources/new-apiResource',
    name: 'New API Resource',
    component: NewApiResource,
    props: true,
    meta: { requiresAuthentication: true, requiredRole: 'idsrvAdmin' },
  },
  {
    path: '/idsrv/apiScopes',
    name: 'List API Scopes',
    component: ListApiScopes,
    props: true,
    meta: { requiresAuthentication: true, requiredRole: 'idsrvAdmin' },
  },
  {
    path: '/idsrv/apiScopes/new-scope',
    name: 'New API Scope',
    component: NewApiScope,
    props: true,
    meta: { requiresAuthentication: true, requiredRole: 'idsrvAdmin' },
  },
  {
    path: '/profile-picture-requests',
    name: 'Profile Picture Requests',
    component: ProfilePictureRequest,
    meta: { requiresAuthentication: true, requiredRole: 'profilePictureReviewer' },
  },
  {
    path: '/profile-picture-review',
    name: 'Profile Picture Review',
    component: ProfilePictureReview,
    meta: { requiresAuthentication: true, requiredRole: 'profilePictureReviewer' },
  },
  {
    path: '/profile-picture-request-generation',
    name: 'Profile Picture Request Generation',
    component: ManualRequestGeneration,
    meta: { requiresAuthentication: true, requiredRole: 'profilePictureReviewer' },
  },
  {
    path: '/profile-picture/:token',
    name: 'Profile Picture Deeplink',
    component: DeeplinkLandingPage,
    props: true,
    meta: { isPublicPage: true },
  },
  {
    path: '/push-notification/development-sender',
    name: 'Push Notification Development Sender',
    component: DevelopmentSender,
    meta: { requiresAuthentication: true, requiredRole: 'pushNotificationDeveloper' },
  },
  {
    path: '/dev/tools',
    name: 'Developer Tools',
    component: DevTools,
    meta: { requiresAuthentication: true, requiredRole: 'developer' },
  },
  {
    path: '/sports-test/leaderboard',
    name: 'Sports Test Leaderboard',
    component: SportsTestLeaderboard,
    meta: { requiresAuthentication: true, requiredRole: 'physicalEducationTeacher' },
  },
  {
    path: '/:pathMatch(.*)*',
    name: 'NotFound',
    component: Home,
  },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

router.beforeEach(async (to) => {
  const authStore = useAuthenticationStore();

  // Load user data if authentication is required and not already loaded
  if (to.meta.requiresAuthentication && !authStore.userData) {
    try {
      await authStore.loadUserData();
    } catch (error) {
      // If loading fails, user is not authenticated
      console.warn('Failed to load user data:', error);
    }
  }

  // Deny access for unauthenticated users when authentication is required.
  if (to.meta.requiresAuthentication && !authStore.isAuthenticated) {
    console.warn('Access denied: Authentication required');
    return { name: 'Home' };
  }

  if (to.meta.requiredRole !== undefined && !authStore.userRoles.includes(to.meta.requiredRole)) {
    console.warn(
      `Access denied: Role '${to.meta.requiredRole}' required. User has roles:`,
      authStore.userRoles
    );
    return { name: 'Home' };
  }
});

export default router;
