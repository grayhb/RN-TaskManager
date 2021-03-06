﻿<template>
    <div class="component-container">

        <v-toolbar flat color="white">
            <v-toolbar-title>{{title}}</v-toolbar-title>
            <v-divider class="mx-4"
                       inset
                       vertical></v-divider>
            <v-spacer></v-spacer>
            <v-dialog v-model="dialog" max-width="500px">
                <template v-slot:activator="{ on, attrs }">
                    <v-btn color="primary"
                           dark
                           class="mb-2"
                           v-bind="attrs"
                           v-on="on">Добавить запись</v-btn>
                </template>
                <v-card>
                    <v-card-title>
                        <span class="headline">Карточка записи</span>
                    </v-card-title>

                    <v-card-text>
                        <v-container>
                            <v-row>
                                <v-col cols="12">
                                    <v-text-field v-model="editedItem.Login" label="Логин"></v-text-field>
                                    <v-text-field v-model="editedItem.Email" label="Email"></v-text-field>
                                    <v-text-field v-model="editedItem.LastName" label="Фамилия"></v-text-field>
                                    <v-text-field v-model="editedItem.FirstName" label="Имя"></v-text-field>
                                    <v-text-field v-model="editedItem.Patronymic" label="Отчество"></v-text-field>
                                    <v-select :items="groups"
                                              item-text="GroupName"
                                              item-value="GroupId"
                                              v-model="editedItem.GroupId"
                                              label="Группа"></v-select>
                                </v-col>
                            </v-row>
                        </v-container>
                    </v-card-text>

                    <v-card-actions>
                        <v-btn color="red darken-1" text @click="deleteItem">Удалить</v-btn>
                        <v-spacer></v-spacer>
                        <v-btn color="blue darken-1" text @click="close">Отмена</v-btn>
                        <v-btn color="blue darken-1" text @click="save">Сохранить</v-btn>
                    </v-card-actions>
                </v-card>
            </v-dialog>
        </v-toolbar>

        <v-data-table :headers="headers"
                      :items="items"
                      :loading="loading"
                      class="cursor-pointer"
                      dense
                      hide-default-footer="true"
                      @click:row="e => editItem(e)"
                      items-per-page="500">

            <template slot="no-data">
                <span>Записей нет</span>
            </template>

        </v-data-table>

        <v-snackbar v-model="snackbar">
            {{ errorMessage }}

            <template v-slot:action="{ attrs }">
                <v-btn color="error"
                       text
                       v-bind="attrs"
                       @click="closeSnackbar">
                    Закрыть
                </v-btn>
            </template>
        </v-snackbar>
    </div>
</template>

<script>
    export default {
        data: () => ({
            title: 'Пользователи',
            dialog: false,
            snackbar: false,
            errorMessage: '',
            items: [],
            groups: [],
            headers: [
                { text: 'Логин', value: 'Login' },
                { text: 'Email', value: 'Email' },
                { text: 'ФИО', value: 'ShortName' },
                { text: 'Группа', value: 'GroupName' },
            ],
            editedIndex: -1,
            editedItem: {
                UserId: 0,
                Login: '',
                LastName: '',
                FirstName: '',
                Patronymic: '',
                Email: '',
                GroupId: -1
            },
            defaultItem: {
                UserId: 0,
                Login: '',
                LastName: '',
                FirstName: '',
                Patronymic: '',
                Email: '',
                GroupId: -1
            },
            loading: false,
            loginBlocked: false,
            api:'/api/users'
        }),
        created() {
            this.loadItems()
        },
        watch: {
            dialog(val) {
                val || this.close()
            },
        },
        methods: {
            fetchData(uri, callback) {

                let self = this;

                return fetch(uri, {
                    credentials: 'include',
                    accept: 'application/json'
                })
                    .then(response => {
                        if (response.status !== 200) {
                            console.log(response)
                            throw new Error(response.status + " - " + response.statusText)
                        }
                        else
                            return response.json()
                    })
                    .then(data => {
                        callback(data);
                        return data;
                    })
                    .catch(ex => {
                        console.log(uri);
                        console.log(ex);
                        self.errorMessage = ex;
                        self.snackbar = true;
                    });
            },
            async loadItems() {

                let self = this;
                this.loading = true;

                await this.fetchData(this.api, (data) => {
                    self.items = data;
                });

                await this.fetchData("/api/groups", (data) => {
                    self.groups = data;
                });

                this.loading = false;
            },
            editItem(item) {
                this.loginBlocked = true;
                this.editedIndex = this.items.indexOf(item)
                this.editedItem = Object.assign({}, item)
                this.dialog = true
            },
            async deleteItem() {

                if (!confirm("Удалить выбранную запись?")) return;

                let self = this;

                await fetch(this.api + '/' + this.editedItem.UserId, {
                    method: 'DELETE',
                    credentials: 'include',
                })
                    .then(async (response) => {

                        if (response.ok) {
                            this.items.splice(this.editedIndex, 1);
                            this.close();
                        }
                        else {
                            if (response.status === 400) {
                                self.errorMessage = await response.text();
                                self.snackbar = true;
                            }
                            else if (response.status === 404) {
                                self.errorMessage = "Запись не найдена";
                                self.snackbar = true;
                            }
                        }

                    });
            },
            close() {
                this.loginBlocked = false;
                this.dialog = false
                this.$nextTick(() => {
                    this.editedItem = Object.assign({}, this.defaultItem)
                    this.editedIndex = -1
                })
            },
            async save() {

                let self = this;
                let needClose = false;
                let method = 'POST';
                const formData = new FormData();

                formData.append('Login', this.editedItem.Login);
                formData.append('LastName', this.editedItem.LastName);
                formData.append('FirstName', this.editedItem.FirstName);
                formData.append('Patronymic', this.editedItem.Patronymic);
                formData.append('Email', this.editedItem.Email);
                formData.append('GroupId', this.editedItem.GroupId);

                // TODO - создание / изменение
                if (this.editedIndex > -1) {
                    method = 'PUT';
                    formData.append('UserId', this.editedItem.UserId);
                }

                await fetch(this.api, {
                    method: method,
                    credentials: 'include',
                    body: formData
                })
                    .then(async (response) => {

                        if (response.ok) {

                            const item = await response.json();

                            if (this.editedIndex > -1)
                                Object.assign(self.items[this.editedIndex], item);
                            else
                                self.items.push(item);

                            needClose = true;
                        }
                        else {
                            if (response.status === 400) {
                                self.errorMessage = await response.text();
                                self.snackbar = true;
                            }
                        }

                    });

                if (needClose)
                    this.close()
            },
            closeSnackbar() {
                this.snackbar = false;
                this.errorMessage = '';
            },
        }
    };
</script>

<style>
</style>