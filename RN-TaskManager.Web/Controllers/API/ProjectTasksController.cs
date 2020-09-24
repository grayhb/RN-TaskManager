using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient.Server;
using RN_TaskManager.DAL.Repositories;
using RN_TaskManager.Models;
using RN_TaskManager.Web.Services;
using RN_TaskManager.Web.ViewModels;

namespace RN_TaskManager.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectTasksController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IProjectTaskRepository _projectTaskRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IProjectTaskTypeRepository _projectTaskTypeRepository;
        private readonly IProjectTaskStatusRepository _projectTaskStatusRepository;
        private readonly IProjectTaskPerformerRepository _projectTaskPerformerRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;

        public ProjectTasksController(
            IMapper mapper,
            IProjectTaskRepository projectTaskRepository,
            IProjectRepository projectRepository,
            IProjectTaskTypeRepository projectTaskTypeRepository,
            IProjectTaskStatusRepository projectTaskStatusRepository,
            IProjectTaskPerformerRepository projectTaskPerformerRepository,
            IGroupRepository groupRepository,
            IUserRepository userRepository,
            IUserService userService)
        {
            _mapper = mapper;
            _projectTaskRepository = projectTaskRepository;
            _projectRepository = projectRepository;
            _projectTaskTypeRepository = projectTaskTypeRepository;
            _projectTaskStatusRepository = projectTaskStatusRepository;
            _projectTaskPerformerRepository = projectTaskPerformerRepository;
            _groupRepository = groupRepository;
            _userRepository = userRepository;
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProjectTaskViewModel>>> GetItems()
        {
            try
            {
                var items = await _projectTaskRepository.ProjectTasksAsync();

                return items.Select(e => _mapper.Map<ProjectTaskViewModel>(e)).ToList();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("my")]
        public async Task<ActionResult<List<ProjectTaskViewModel>>> GetUserTasks()
        {
            try
            {
                var user = _userRepository.FindAsync(e => e.Login.Equals(_userService.userLogin)).Result.FirstOrDefault();
                var items = await _projectTaskRepository.ProjectTasksByUserIdAsync(user.UserId);

                return items.Select(e => _mapper.Map<ProjectTaskViewModel>(e)).ToList();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectTask>> GetItem(int id)
        {
            try
            {
                var item = await _projectTaskRepository.FindByIdAsync(id);

                if (item == null)
                    return NotFound();
                else
                    return item;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<ProjectTaskViewModel>> CreateItem([FromForm] ProjectTaskViewModel item)
        {
            try
            {
                if (item.ProjectTaskId > 0)
                    return BadRequest("Идентификатор записи должен быть равен 0");

                var project = await ProjectByIdAsync(item.ProjectId);

                if (project == null)
                    return BadRequest("Выбранный проект не найден");

                var group = await GroupByIdAsync(item.GroupId);

                if (group == null)
                    return BadRequest("Выбранная группа не найдена");

                var taskType = await ProjectTaskTypeByIdAsync(item.ProjectTaskTypeId);

                if (taskType == null)
                    return BadRequest("Выбранный тип задачи не найден");

                var taskStatus = await ProjectTaskStatusByIdAsync(item.ProjectTaskStatusId);

                if (taskStatus == null)
                    return BadRequest("Выбранный статус не найден");

                var newItem = _mapper.Map<ProjectTask>(item);

                newItem.Project = project;
                newItem.Group = group;
                newItem.TaskStatus = taskStatus;
                newItem.TaskType = taskType;

                newItem.DateCreated = DateTime.Now;

                var userCreated = (await _userRepository.FindAsync(e => !e.Deleted && e.Login.ToLower().Equals(_userService.userLogin.ToLower()))).SingleOrDefault();
                newItem.LoginCreated = userCreated != null ? userCreated.ShortName : _userService.userLogin;

                newItem.ProjectTaskPerformers = new List<ProjectTaskPerformer>();

                if (!string.IsNullOrEmpty(item.Users))
                {
                    var userIds = item.Users.Split(",").Select(e => int.Parse(e)).ToList();
                    foreach (int userId in userIds)
                    {
                        var user = await _userRepository.FindByIdAsync(userId);
                        newItem.ProjectTaskPerformers.Add(new ProjectTaskPerformer()
                        {
                            User = user
                        });
                    }
                }

                await _projectTaskRepository.CreateAsync(newItem);

                return _mapper.Map<ProjectTaskViewModel>(newItem);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult<ProjectTaskViewModel>> UpdateItem([FromForm] ProjectTaskViewModel item)
        {
            try
            {
                var existItem = await _projectTaskRepository.ProjectTaskByIdAsync(item.ProjectTaskId);

                if (existItem == null)
                    return NotFound();

                var project = await ProjectByIdAsync(item.ProjectId);

                if (project == null)
                    return BadRequest("Выбранный проект не найден");

                var group = await GroupByIdAsync(item.GroupId);

                if (group == null)
                    return BadRequest("Выбранная группа не найдена");

                var taskType = await ProjectTaskTypeByIdAsync(item.ProjectTaskTypeId);

                if (taskType == null)
                    return BadRequest("Выбранный тип задачи не найден");

                var taskStatus = await ProjectTaskStatusByIdAsync(item.ProjectTaskStatusId);

                if (taskStatus == null)
                    return BadRequest("Выбранный статус не найден");

                existItem.Project = project;
                existItem.Group = group;
                existItem.TaskStatus = taskStatus;
                existItem.TaskType = taskType;

                existItem.Details = item.Details;
                existItem.Note = item.Note;
                existItem.Priority = item.Priority;
                existItem.DurationHours = item.DurationHours;

                existItem.StartPlan = item.StartPlan;
                existItem.EndPlan = item.EndPlan;

                existItem.StartFact = item.StartFact;
                existItem.EndFact = item.EndFact;

                existItem.DateEdited = DateTime.Now;

                var userEdited = (await _userRepository.FindAsync(e => !e.Deleted && e.Login.ToLower().Equals(_userService.userLogin.ToLower()))).SingleOrDefault();
                existItem.LoginEdited = userEdited != null ? userEdited.ShortName : _userService.userLogin;

                if (!string.IsNullOrEmpty(item.Users))
                {
                    if (existItem.ProjectTaskPerformers == null)
                        existItem.ProjectTaskPerformers = new List<ProjectTaskPerformer>();

                    var userIds = item.Users.Split(",").Select(e => int.Parse(e)).ToList();

                    foreach (var p in existItem.ProjectTaskPerformers)
                    {
                        // пользователя нет в новых данных
                        if (!userIds.Any(e => e == p.UserId))
                            p.Deleted = true;
                    }

                    foreach (int userId in userIds)
                    {
                        // проверяем 
                        if (!existItem.ProjectTaskPerformers.Any(e => e.UserId.Equals(userId) && !e.Deleted))
                        {
                            var user = await _userRepository.FindByIdAsync(userId);
                            if (user != null)
                            {
                                existItem.ProjectTaskPerformers.Add(new ProjectTaskPerformer()
                                {
                                    User = user
                                });
                            }
                        }
                    }

                }
                else if (existItem.ProjectTaskPerformers != null && existItem.ProjectTaskPerformers.Count > 0)
                {
                    foreach (var p in existItem.ProjectTaskPerformers)
                        p.Deleted = true;
                }

                await _projectTaskRepository.EditAsync(existItem);

                return _mapper.Map<ProjectTaskViewModel>(existItem);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            try
            {
                var item = await _projectTaskRepository.FindByIdAsync(id);

                if (item == null)
                    return NotFound();
                else
                {
                    item.Deleted = true;

                    item.DateDeleted = DateTime.Now;

                    var userDeleted = (await _userRepository.FindAsync(e => !e.Deleted && e.Login.ToLower().Equals(_userService.userLogin.ToLower()))).SingleOrDefault();
                    item.LoginDeleted = userDeleted != null ? userDeleted.ShortName : _userService.userLogin;

                    await _projectTaskRepository.EditAsync(item);

                    // удалить всех исполнителей
                    var performers = await _projectTaskPerformerRepository.FindAsync(e => e.ProjectTaskId.Equals(id) && !e.Deleted);

                    foreach (var performer in performers)
                        performer.Deleted = true;

                    await _projectTaskPerformerRepository.EditAsync(performers.ToList());

                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        async Task<Project> ProjectByIdAsync(int id) => id > 0 ? await _projectRepository.FindByIdAsync(id) : null;

        async Task<Group> GroupByIdAsync(int? id) => id > 0 ? await _groupRepository.FindByIdAsync(id.Value) : null;

        async Task<ProjectTaskType> ProjectTaskTypeByIdAsync(int? id) => id > 0 ? await _projectTaskTypeRepository.FindByIdAsync(id.Value) : null;

        async Task<ProjectTaskStatus> ProjectTaskStatusByIdAsync(int? id) => id > 0 ? await _projectTaskStatusRepository.FindByIdAsync(id.Value) : null;

    }
}
