import Vue from 'vue';
import VueRouter from 'vue-router';
import Home from './components/home.vue';
import SignIn from './components/sign-in.vue';
import Search from './components/search.vue';
import UserList from './components/user-list.vue';
import User from './components/user.vue';

Vue.use(VueRouter);

export default new VueRouter({
  routes: [
    {
      path: '/',
      redirect: 'Home'
    },
    {
      path: '/home',
      name: 'Home',
      component: Home
    },
    {
      path: '/sign-in',
      name: 'Sign In',
      component: SignIn
    },
    {
      path: '/search',
      name: 'Search',
      component: Search
    },
    {
      path: '/users',
      component: UserList,
      name: 'User Administration',
      meta: { adminView: true }
    },
    {
      path: '/user/:id([0-9a-f]{8}-?[0-9a-f]{4}-?[1-5][0-9a-f]{3}-?[89ab][0-9a-f]{3}-?[0-9a-f]{12})',
      component: User,
      name: 'User',
      meta: { adminView: true },
      props: true
    },
    {
      path: '*',
      redirect: 'Home'
    }
  ]
});
