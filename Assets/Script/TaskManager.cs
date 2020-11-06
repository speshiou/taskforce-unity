using UnityEngine;
using System.Collections;

public class TaskManager : MonoBehaviour {
	
	public GameManager gameManager;
	private Task _currentTask;
	public TextMesh textCurrentTaskLevel;
	public TextMesh textNextTaskLevel;
	public TextMesh textCurrentTaskDescription;
	public TextMesh textNextTaskDescription;
	
	public bool isCurrentTaskComplete;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void setCurrentLevel(int level) {
		Task task = findLevel(level);
		if(task != null)
			_currentTask = task;
	}
	
	public Task findLevel(int level) {
		Task[] tasks = gameObject.GetComponentsInChildren<Task>();
		foreach (Task task in tasks) {
			if(task.level == level) {
				return task;
			}
		}
		return null;
	}
	
	public Task getCurrentTask() {
		return _currentTask;
	}
	
	public int getCurrentLevel() {
		return _currentTask.level;
	}

	public bool isComplete(Task task) {
		if(task.distance <= gameManager.distance) {
			return true;
		}
		return false;
	}
	
	public void updateLevel() {
		/*isCurrentTaskComplete = false;
		if(_currentTask.score <= gameManager.score
			&& _currentTask.distance <= gameManager.distance
			&& _currentTask.kill <= gameManager.kill
			&& _currentTask.headshot <= gameManager.headshot) {
			isCurrentTaskComplete = true;
		}
		//textCurrentTaskLevel.text = _currentTask.level +  ".";
		textCurrentTaskDescription.text = _currentTask.description;
		Task nextTask = findLevel(_currentTask.level + 1);
		if(nextTask != null) {
			//textNextTaskLevel.text = nextTask.level +  ".";	
			textNextTaskDescription.text = nextTask.description;
		}
		
		if(isCurrentTaskComplete) {
			setCurrentLevel(getCurrentLevel() + 1);
			
			if(gameManager.rank < _currentTask.rank) {
				gameManager.rank = _currentTask.rank;
			}
		}*/
	}
}
