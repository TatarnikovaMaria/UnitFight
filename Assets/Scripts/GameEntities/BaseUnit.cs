using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EpPathFinding.cs;

public class BaseUnit : MonoBehaviour, IDamagable
{
    protected int lifes = 5;
    protected float moveSpeed = 5f;
    protected float rotationSpeed = 10f;
    protected float attackDistance = 1;
    protected int attackDamage = 1;
    protected float attackDelay = 0.3f;
    protected UnitTeams myTeam;

    private float checkEnemyDelay = 1f;
    private float checkTimer = 0;

    private static Dictionary<UnitTeams, List<BaseUnit>> teamMembers = new Dictionary<UnitTeams, List<BaseUnit>>();

    private float eps = 0.01f;
    private Transform _transform;
    protected BaseUnit targetEnemy;
    private Vector3 targetSavePosition;
    private List<Vector3> path = new List<Vector3>();
    private int pathInd = 1;

    public static void ActivateAllUnits()
    {
        foreach (var item in teamMembers)
        {
            for (int i = 0; i < item.Value.Count; i++)
            {
                item.Value[i].Activate();
            }
        }
    }

    public virtual void Init(UnitSettings settings)
    {
        this.lifes = settings.lifes;
        this.moveSpeed = settings.moveSpeed;
        this.rotationSpeed = settings.rotationSpeed;
        this.attackDistance = settings.attackDistance;
        this.attackDamage = settings.attackDamage;
        this.attackDelay = settings.attackDelay;
        this.myTeam  = settings.team == "People" ? UnitTeams.people : UnitTeams.zombie;
        AddToList();
    }

    private void AddToList()
    {
        List<BaseUnit> myTeamMembersList;
        if(teamMembers.ContainsKey(myTeam))
        {
            myTeamMembersList = teamMembers[myTeam];
        }
        else
        {
            myTeamMembersList = new List<BaseUnit>();
            teamMembers.Add(myTeam, myTeamMembersList);
        }
        myTeamMembersList.Add(this);
        UIManager.instance.SetTeamCount(myTeam, myTeamMembersList.Count);

        _transform = transform;    
    }

    private void OnDestroy()
    {
        teamMembers[myTeam].Remove(this);
        UIManager.instance.SetTeamCount(myTeam, teamMembers[myTeam].Count);

        if (teamMembers[myTeam].Count == 0)
        {
            teamMembers.Remove(myTeam);
            if(teamMembers.Count < 2)
            {
                GameManager.instance.GameStatus = GameStatus.GameOver;
            }
        }
    }

    public void GetDamage(int damage)
    {
        lifes -= damage;
        if(lifes <= 0)
        {
            Destroy(gameObject);
        }
    }

    public virtual void Activate()
    {
        StartCoroutine(FindMoveAttack());
    }

    protected virtual void Attack()
    {
        if (targetEnemy != null)
        {
            IDamagable damagable = targetEnemy.GetComponent<IDamagable>();
            if (damagable != null)
            {
                damagable.GetDamage(attackDamage);
            }
        }
    }

    private void Update()
    {
        checkTimer -= Time.deltaTime;
    }

    private IEnumerator FindMoveAttack()
    {
        WaitForSeconds checkWait = new WaitForSeconds(checkEnemyDelay);
        WaitForSeconds attackWait = new WaitForSeconds(attackDelay);
        Vector3 moveDirection;
        Quaternion lookRotation;

        while (GameManager.instance.GameStatus == GameStatus.Game)
        {
            if (targetEnemy == null)                
            {
                targetEnemy = FindNearestEnemy();
                if(targetEnemy == null)
                {
                    yield return checkWait;
                }
            }
            else if (Vector3.Distance(_transform.position, targetEnemy.transform.position) <= attackDistance)
            {
                yield return StartCoroutine(LookToTarget(targetEnemy.transform.position));
                Attack();
                yield return attackWait;
            }
            else if (checkTimer <= 0 && Vector3.Distance(targetEnemy.transform.position, targetSavePosition) > LvlManager.instance.WorldCellSize)
            {
                targetSavePosition = targetEnemy.transform.position;
                path = LvlManager.instance.FindPath(_transform.position, targetSavePosition);
                pathInd = 1;
                if (path.Count == 0)
                {
                    yield return checkWait;
                }
                else
                {
                    checkTimer = checkEnemyDelay;           //to avoid find new path too often. Timer is changing in update
                }
            }
            else if(pathInd < path.Count)
            {
                if (Vector3.Distance(_transform.position, path[pathInd]) < LvlManager.instance.WorldCellSize / 2f)
                {
                    pathInd++;
                }
                else
                {
                    moveDirection = (path[pathInd] - _transform.position).normalized;
                    lookRotation = Quaternion.LookRotation(moveDirection);

                    if (Quaternion.Angle(_transform.rotation, lookRotation) > 1)
                    {
                        _transform.rotation = Quaternion.Lerp(_transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
                        yield return null;
                    }
                    else if (Vector3.Distance(_transform.position, path[pathInd]) > eps)
                    {
                        _transform.position += moveDirection * moveSpeed * Time.deltaTime;
                        yield return null;
                    }
                    else
                    {
                        pathInd++;
                    }
                }
            }
            else
            {
                yield return null;
            }
         }
    }

    private BaseUnit FindNearestEnemy()
    {
        float minDistance = float.MaxValue;
        float curDistance;
        BaseUnit targetEnemy = null;

        foreach(var item in teamMembers)
        {
            if(item.Key != myTeam)
            {
                for (int i = 0; i < item.Value.Count; i++)
                {
                    curDistance = Vector3.Distance(_transform.position, item.Value[i].transform.position);
                    if (curDistance < minDistance)
                    {
                        minDistance = curDistance;
                        targetEnemy = item.Value[i];
                    }
                }
            }
        }
        return targetEnemy;
    }

    private IEnumerator LookToTarget(Vector3 targetPosition)
    {
        Quaternion lookRotation = Quaternion.LookRotation(targetPosition - _transform.position);
        while (Quaternion.Angle(_transform.rotation, lookRotation) > 1)
        {
            _transform.rotation = Quaternion.Lerp(_transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
