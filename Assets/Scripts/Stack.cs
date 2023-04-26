using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Stack : MonoBehaviour
{
    [SerializeField] TMP_Text stackTitle;
    [SerializeField] GameObject blockPrefab;
    [SerializeField] Material[] masteryMaterials;
    [SerializeField] List<GameObject> blocks;
    [SerializeField] bool testingStack;

    public void SetTitle(string title)
    {
        if(stackTitle != null)
            stackTitle.text = title;
    }

    public string GetTitle()
    {
        return stackTitle.text;
    }

    [ContextMenu("Reconstruct Stack")]
    public void ResetStack()
    {
        if (!testingStack)
            return;

        int index = 0;
        foreach(GameObject block in blocks)
        {
            block.SetActive(true);
            block.GetComponent<Rigidbody>().isKinematic = true;

            bool even = (index / 3) % 2 == 1;
            if (even)
                block.transform.rotation = Quaternion.Euler(0, 90, 0);
            else
                block.transform.rotation = Quaternion.identity;

            block.transform.position = GetBlockPosition(index,even);

            index++;
        }

        testingStack = false;
    }

    [ContextMenu("Test my Stack")]
    public void ActivatePhysics()
    {
        if (testingStack)
            return;

        testingStack = true;

        foreach(GameObject block in blocks)
        {
            if (block.GetComponent<Block>().mastery == 0)
                block.SetActive(false);
            else
                block.GetComponent<Rigidbody>().isKinematic = false;
        }
    }

    public void AddBlock(BlockData data)
    {
        Vector3 blockPos;
        Quaternion blockRot = Quaternion.identity;
        bool even = false;

        // Define rotation based on the stack level
        if ((blocks.Count / 3) % 2 == 1)
        {
            blockRot = Quaternion.Euler(0, 90, 0);
            even = true;
        }

        blockPos = GetBlockPosition(blocks.Count, even);

        // Instantiates block and sets its values
        Block block = Instantiate(blockPrefab, blockPos, blockRot, transform).GetComponent<Block>();
        block.mastery = data.mastery;
        block.grade = data.grade;
        block.domain = data.domain;
        block.cluster = data.cluster;
        block.standardID = data.standardid;
        block.standardDescription = data.standarddescription;

        block.GetComponent<MeshRenderer>().material = masteryMaterials[block.mastery];

        switch(block.mastery)
        {
            case 0:
                block.transform.GetComponentInChildren<TMP_Text>().text = "";
                break;
            case 1:
                block.transform.GetComponentInChildren<TMP_Text>().text = "Learned";
                break;
            case 2:
                block.transform.GetComponentInChildren<TMP_Text>().text = "Mastered";
                break;
        }
        
        blocks.Add(block.gameObject);
    }

    Vector3 GetBlockPosition(int index, bool even)
    {
        Vector3 result = transform.position;

        // Define Y postion
        result.y += 1 + (index / 3);

        // Define X position or Z position, depending on the stack level
        switch ((index + 1) % 3)
        {
            case 1:
                if (even)
                    result.z = transform.position.z - 2.1f;
                else
                    result.x = transform.position.x - 2.1f;
                break;
            case 2:
                if (even)
                    result.z = transform.position.z;
                else
                    result.x = transform.position.x;
                break;
            case 0:
                if (even)
                    result.z = transform.position.z + 2.1f;
                else
                    result.x = transform.position.x + 2.1f;
                break;
        }

        return result;
    }
}
