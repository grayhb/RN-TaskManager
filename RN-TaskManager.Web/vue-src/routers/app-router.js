import Router from "vue-router";
import notFound from '../not-found.vue';
import groups from "../components/groups";
import users from "../components/users";
import projectTaskStatus from "../components/projectTaskStatus";
import taskTypes from "../components/task-types";
import projects from "../components/projects";
import projectTasks from "../components/project-tasks";


Vue.use(Router);

export default new Router({
    routes: [
        //default route redirection
        {
            path: "/",
            redirect: { name: "project-tasks" },
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
            path: "/project-tasks/:task_id",
            name: "project-tasks",
            component: projectTasks,
            display: true,
            label: "Задачи",
            panel: "Задачи",
            icon: "mdi-database"
        },
        {
            path: "/projects",
            name: "projects",
            component: projects,
            display: true,
            label: "Проекты",
            panel: "Справочники",
            icon: "mdi-briefcase"
        },
        {
            path: "/taskTypes",
            name: "taskTypes",
            component: taskTypes,
            display: true,
            label: "Типы работ",
            panel: "Справочники",
            icon: "mdi-label"
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