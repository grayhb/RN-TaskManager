import Router from "vue-router";
import notFound from '../not-found.vue';
import groups from "../components/groups";
import users from "../components/users";
import projectTaskStatus from "../components/projectTaskStatus";


Vue.use(Router);

export default new Router({
    routes: [
        //default route redirection
        {
            path: "/",
            redirect: { name: "users" },
            display: false
        },
        //not found route redirection
        {  
            path: "*",
            component: notFound,
            display: false
        },
        { 
            path: "/not-found",
            name: "not Found",
            component: notFound,
            display: false
        },
        {
            path: "/projectTaskStatus",
            name: "projectTaskStatus",
            component: projectTaskStatus,
            display: true,
            label: "Статусы работ",
            panel: "Справочники",
            icon: "mdi-flag-variant"
        },
        {
            path: "/users",
            name: "users",
            component: users,
            display: true,
            label: "Пользователи",
            panel: "Справочники",
            icon: "mdi-account"
        },
        {
            path: "/groups",
            name: "groups",
            component: groups,
            display: true,
            label: "Группы",
            panel: "Справочники",
            icon: "mdi-account-group"
        },
        
       
    ]
});