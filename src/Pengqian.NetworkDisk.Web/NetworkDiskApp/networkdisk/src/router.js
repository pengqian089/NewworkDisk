import Vue from "vue";
import Router from "vue-router";
import login from './components/login'
import NetworkDisk from './components/NetworkDisk'

Vue.use(Router);

export default new Router({
    routes: [
        {path: "/login", component: login, name: "login"},
        {path: "/", component: NetworkDisk, name: "network disk", meta: {auth: true}}
    ]
});