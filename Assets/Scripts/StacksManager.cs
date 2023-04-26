using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class StacksManager : MonoBehaviour
{
    [SerializeField] BlockData[] blocks;
    [SerializeField] List<Stack> stacks;
    [SerializeField] int currentlyFocusedStack = 0;
    [SerializeField] GameObject stackPrefab;
    public int stackDistance = 10;

    private void Start()
    {
        StartCoroutine(GetRequest("https://ga1vqcu3o1.execute-api.us-east-1.amazonaws.com/Assessment/stack"));
    }

    public bool IsBlockInCurrentStack(string grade)
    {
        return stacks[currentlyFocusedStack].GetTitle().Equals(grade);
    }

    public void CameraToPreviousStack()
    {
        if (currentlyFocusedStack == 0)
            return;

        ResetStack();

        currentlyFocusedStack--;
        Camera.main.GetComponent<CameraController>().ChangeTarget(stacks[currentlyFocusedStack].transform);

        // Disables Prev Button, if can't go back any further
        if (currentlyFocusedStack == 0)
            GameObject.FindGameObjectWithTag("UI Handler").GetComponent<UIHandler>().prevBtn.interactable = false;
        // Re-enables Next Button
        GameObject.FindGameObjectWithTag("UI Handler").GetComponent<UIHandler>().nextBtn.interactable = true;
    }

    public void CameraToNextStack()
    {
        if (currentlyFocusedStack == stacks.Count-1)
            return;

        ResetStack();

        currentlyFocusedStack++;
        Camera.main.GetComponent<CameraController>().ChangeTarget(stacks[currentlyFocusedStack].transform);

        // Disables Next Button, if can't go forward any further
        if (currentlyFocusedStack == stacks.Count-1)
            GameObject.FindGameObjectWithTag("UI Handler").GetComponent<UIHandler>().nextBtn.interactable = false;
        // Re-enables Prev Button
        GameObject.FindGameObjectWithTag("UI Handler").GetComponent<UIHandler>().prevBtn.interactable = true;
    }

    public void TestMyStack()
    {
        stacks[currentlyFocusedStack].ActivatePhysics();
        GameObject.FindGameObjectWithTag("UI Handler").GetComponent<UIHandler>().testBtn.interactable = false;
        GameObject.FindGameObjectWithTag("UI Handler").GetComponent<UIHandler>().resetBtn.interactable = true;
    }

    public void ResetStack()
    {
        stacks[currentlyFocusedStack].ResetStack();
        GameObject.FindGameObjectWithTag("UI Handler").GetComponent<UIHandler>().resetBtn.interactable = false;
        GameObject.FindGameObjectWithTag("UI Handler").GetComponent<UIHandler>().testBtn.interactable = true;
    }

    IEnumerator GetRequest(string url)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            switch (request.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError($"Error: {request.error}");
                    break;
                case UnityWebRequest.Result.Success:
                    blocks = JsonConvert.DeserializeObject<BlockData[]>(request.downloadHandler.text);

                    InstantiateStacks(blocks);

                    break;
            }
        }
    }

    void InstantiateStacks(BlockData[] blocks)
    {
        List<string> auxNames = new List<string>();
        List<List<BlockData>> auxStacks = new List<List<BlockData>>();

        int index = 0;

        // Parse through all blocks
        foreach (BlockData block in blocks)
        {
            if (!auxNames.Contains(block.grade)) // Instantiate a new stack
            {
                // Doesn't create the forth stack (algebra)
                if (auxNames.Count >= 3)
                    continue;

                GameObject g = Instantiate(stackPrefab, transform.position + new Vector3(stackDistance * stacks.Count, 0, 0), Quaternion.identity);
                stacks.Add(g.GetComponent<Stack>());
                g.GetComponent<Stack>().SetTitle(block.grade);
                g.name = $"Stack {block.grade}";

                auxNames.Add(block.grade);
                auxStacks.Add(new List<BlockData>());

                index = stacks.Count - 1;
            }
            else // Current grade stack already exists
            {
                for(int i = 0; i < stacks.Count; i++)
                {
                    Stack s = stacks[i];
                    if (block.grade.Equals(s.name))
                    {
                        index = i;
                    }
                }
            }

            // Add block to auxiliar list
            auxStacks[index].Add(block);
        }

        index = 0;
        // Sorts each auxiliar list and adds each block to the stacks
        foreach (List<BlockData> blockDatas in auxStacks)
        {
            blockDatas.Sort((x, y) =>
            {
                int res = x.domain.CompareTo(y.domain);
                if (res == 0)
                    res = x.cluster.CompareTo(y.cluster);
                if (res == 0)
                    res = x.standardid.CompareTo(y.standardid);
                return res;
            });

            foreach (BlockData b in blockDatas)
                stacks[index].AddBlock(b);

            index++;
        }

        // Set camera's target to the first stack
        Camera.main.GetComponent<CameraController>().ChangeTarget(stacks[0].transform);
    }
}
