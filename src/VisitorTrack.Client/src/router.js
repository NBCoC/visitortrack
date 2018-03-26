import Vue from 'vue';
import VueRouter from 'vue-router';
import Home from './components/home';
import SignIn from './components/sign-in';
import Search from './components/search';
import Users from './components/users';

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
      component: Users,
      name: 'User Administration',
      meta: { adminView: true }
    },
    {
      path: '*',
      redirect: 'Home'
    }
  ]
});
